using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RequestReceiver.Messaging.Send
{
    public class RequestSenderRpc : IRequestSenderRpc
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly string _queuename;
        
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> 
            _callbackMapper = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        public RequestSenderRpc(IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMqOptions.Value.Hostname,
                UserName = rabbitMqOptions.Value.UserName,
                Password = rabbitMqOptions.Value.Password,
                Port = 5672
            };
            
            _queuename = rabbitMqOptions.Value.QueueName;
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += OnReceived;
        }
        
        public Task<string> CallAsync(object request,
            CancellationToken cancellationToken = default)
        {

            var correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<string>();
            _callbackMapper.TryAdd(correlationId, tcs);
            
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = _replyQueueName;
            
            var json = JsonConvert.SerializeObject(request);
            var messageBytes = Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish("", _queuename, props, messageBytes);
            _channel.BasicConsume(consumer: _consumer, queue: _replyQueueName, autoAck: true);

            cancellationToken.Register(() => 
                _callbackMapper.TryRemove(correlationId, out _));
            return tcs.Task;
        }
        
        public void Close() => _connection.Close();

        private void OnReceived(object model, BasicDeliverEventArgs ea)
        {
            var suchTaskExists = _callbackMapper.TryRemove(ea.BasicProperties.CorrelationId, 
                out var tcs);
            
            if (!suchTaskExists) return;
            
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            
            tcs.TrySetResult(response);
            
            Console.WriteLine(" [.] Got '{0}'", response);
        }
    }
}