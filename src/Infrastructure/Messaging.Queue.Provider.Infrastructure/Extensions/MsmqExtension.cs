using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Infrastructure.Extensions
{
    public static class MsmqExtension
    {
        public static Task<System.Messaging.Message> ReceiveAsync(this MessageQueue messageQueue)
        {
            return Task.Factory.FromAsync(messageQueue.BeginReceive(), r => messageQueue.EndReceive(r));
        }

        public static Task<System.Messaging.Message> ReceiveAsync(this MessageQueue messageQueue, TimeSpan timeout)
        {
            return Task.Factory.FromAsync(messageQueue.BeginReceive(timeout), r => messageQueue.EndReceive(r));
        }

        public static Task<System.Messaging.Message> PeekAsync(this MessageQueue messageQueue)
        {
            return Task.Factory.FromAsync(messageQueue.BeginPeek(), r => messageQueue.EndPeek(r));
        }

        public static Task<System.Messaging.Message> PeekAsync(this MessageQueue messageQueue, TimeSpan timeout)
        {
            return Task.Factory.FromAsync(messageQueue.BeginPeek(timeout), r => messageQueue.EndPeek(r));
        }
        public static Task PurgeAsync(this MessageQueue messageQueue)
        {
            messageQueue.Purge();
            return Task.FromResult<object>(null);
        }
    }
}
