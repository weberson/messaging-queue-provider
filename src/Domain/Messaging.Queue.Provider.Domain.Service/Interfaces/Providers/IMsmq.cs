using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Interfaces.Providers
{
    public interface IMsmq<T> : IMessageQueue<T> where T : class
    {
        Task PushAsync(QueueMessage<T> message, MessagePriority priority);

        Task<string> PushAsync(int correlationId, string body, MessagePriority priority);

        Task<string> PushAsync(int correlationId, string label, string body, MessagePriority priority);

        Task PurgeAsync();
    }
}
