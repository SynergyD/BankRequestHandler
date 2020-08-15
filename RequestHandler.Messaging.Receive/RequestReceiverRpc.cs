using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RequestHandler.DataAccess.Entities;
using RequestHandler.DataAccess.Repositories;

namespace RequestHandler.Messaging.Receive
{
    public class RequestReceiverRpc : IRequestReceiverRpc
    {
        private static IModel _channel;
        
        public RequestReceiverRpc()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "root",
                Password = "root",
                DispatchConsumersAsync = true
            };

            using var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare("RequestQueue", false, false, false, null);
            _channel.BasicQos(0, 1, false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnReceived;

            _channel.BasicConsume("RequestQueue", false, consumer);

            Console.WriteLine(" [x] Awaiting RPC requests");
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
        
        private static async Task OnReceived(object model, BasicDeliverEventArgs ea)
        {
            string response = null;

            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;
            
            try
            {
                var message = Encoding.UTF8.GetString(body);
                RequestRepository repository = new RequestRepository();

                if (int.TryParse(message,out int result))
                {
                    response = await Task.Run(() => JsonConvert.SerializeObject(repository.Get(result)));
                }

                else if(message.Length > 90)
                {
                    var request = JsonConvert.DeserializeObject<Request>(message);
                    response = await Task.Run(() => repository.Save(request).ToString());
                }
                
                else
                {
                    var request = JsonConvert.DeserializeObject<RequestByClient>(message);
                    response = await Task.Run(() => JsonConvert.SerializeObject(repository.GetByClient(request)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" [.] " + e.Message);
                response = "";
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(response);
                
                _channel.BasicPublish("", props.ReplyTo, replyProps, responseBytes);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
        }
    }
}