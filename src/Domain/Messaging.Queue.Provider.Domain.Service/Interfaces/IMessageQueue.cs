using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Interfaces
{
    public interface IMessageQueue<T> : IQueue<QueueMessage<T>> where T : class
    {
        IMessageSerializer<T> MessageSerializer { get; set; }

        Task<string> PushAsync(int correlationId, string body);

        Task<string> PushAsync(int correlationId, string label, string body);
    }
}
