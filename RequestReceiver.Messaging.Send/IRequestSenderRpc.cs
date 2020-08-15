using System.Threading;
using System.Threading.Tasks;

namespace RequestReceiver.Messaging.Send
{
    public interface IRequestSenderRpc
    {
        public Task<string> CallAsync(object request,
            CancellationToken cancellationToken = default);
        public void Close();
    }
}