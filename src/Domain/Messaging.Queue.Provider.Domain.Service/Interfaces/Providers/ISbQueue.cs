using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Interfaces.Providers
{
    public interface ISbQueue<T> : IMessageQueue<T> where T : class
    {
        Task<QueueMessage<T>> PeekAsync(long sequenceNumber);
    }
}
