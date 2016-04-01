using Messaging.Queue.Provider.Domain.Enum;
using Messaging.Queue.Provider.Domain.Service.Interfaces;
using Messaging.Queue.Provider.Domain.Service.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Factories
{
    public interface IMessageQueueProviderFactory
    {
        IMsmq<T> CreateMsmq<T>(Serializer serializer, string queueName) where T : class;

        ISbQueue<T> CreateSbQueue<T>(Serializer serializer, string queueName) where T : class;
    }

    public abstract class AbstractMessageQueueProviderFactory : IMessageQueueProviderFactory
    {
        #region IMessageQueueProviderFactory

        public abstract IMsmq<T> CreateMsmq<T>(Serializer serializer, string queueName) where T : class;

        public abstract ISbQueue<T> CreateSbQueue<T>(Serializer serializer, string queueName) where T : class;

        #endregion
    }
}
