using Messaging.Queue.Provider.Domain.Enum;
using Messaging.Queue.Provider.Domain.Service.Factories;
using Messaging.Queue.Provider.Domain.Service.Interfaces;
using Messaging.Queue.Provider.Domain.Service.Interfaces.Providers;
using Messaging.Queue.Provider.Infrastructure.Providers;
using Messaging.Queue.Provider.Infrastructure.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Infrastructure.Factories
{
    public class MessageQueueProviderFactory : AbstractMessageQueueProviderFactory
    {
        #region Private Fields

        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region Constructor

        public MessageQueueProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region IMessageQueueFactory Members

        public override IMsmq<T> CreateMsmq<T>(Serializer serializer, string queueName)
        {
            var messageQueue = _serviceProvider.GetService(typeof(Msmq<T>)) as IMsmq<T>;

            if (messageQueue == null)
            {
                throw new Exception("ServiceProvider get 'IMessageQueue<T>' service error.");
            }

            switch (serializer)
            {
                case Serializer.Json:
                    messageQueue.MessageSerializer = _serviceProvider.GetService(typeof(JsonSerializer<T>)) as IMessageSerializer<T>;
                    break;

                default:
                    throw new Exception("MsmqProvider get 'IMessageSerializer<T>' service error.");
            }

            messageQueue.Initializer(queueName);

            return messageQueue;
        }

        public override ISbQueue<T> CreateSbQueue<T>(Serializer serializer, string queueName)
        {
            var messageQueue = _serviceProvider.GetService(typeof(Msmq<T>)) as ISbQueue<T>;

            if (messageQueue == null)
            {
                throw new Exception("ServiceProvider get 'IMessageQueue<T>' service error.");
            }

            switch (serializer)
            {
                case Serializer.Json:
                    messageQueue.MessageSerializer = _serviceProvider.GetService(typeof(JsonSerializer<T>)) as IMessageSerializer<T>;
                    break;

                default:
                    throw new Exception("SbQueueProvider get 'IMessageSerializer<T>' service error.");
            }

            messageQueue.Initializer(queueName);

            return messageQueue;
        }

        #endregion
    }
}
