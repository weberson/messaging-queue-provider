using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Interfaces
{
    public interface IQueue<TMessage> : IDisposable
    {
        void Initializer(string queueName);

        Task PushAsync(TMessage message);

        Task<TMessage> ReceiveAsync();

        Task<TMessage> ReceiveAsync(TimeSpan timeout);

        Task<TMessage> PeekAsync();
        
        void Close();

        string QueueName { get; }
    }
}
