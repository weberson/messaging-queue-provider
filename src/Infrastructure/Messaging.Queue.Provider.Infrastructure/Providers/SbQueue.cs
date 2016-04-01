using Messaging.Queue.Provider.Domain.Service;
using Messaging.Queue.Provider.Domain.Service.Interfaces;
using Messaging.Queue.Provider.Domain.Service.Interfaces.Providers;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Infrastructure.Providers
{
    /// <summary>
    /// Barramento de serviço do azure
    /// </summary>
    public class SbQueue<T> : ISbQueue<T> where T : class
    {
        #region Private Fields

        private IMessageSerializer<T> _messageSerializer;

        private QueueClient _queueClient;

        private string _queueName;

        private string _sbConnectionString;

        private static object _syncRoot = new object();

        #endregion

        #region Constructor

        public SbQueue()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create queue if not exists.
        /// </summary>
        private void CheckCreateQueue(string queueName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_sbConnectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                lock (_syncRoot)
                {
                    if (!namespaceManager.QueueExists(queueName))
                    {
                        namespaceManager.CreateQueue(queueName);
                    }
                }
            }
        }

        #endregion

        #region IMessageQueue Members

        public async Task<string> PushAsync(int correlationId, string body)
        {
            return await PushAsync(correlationId, string.Empty, body);
        }

        public async Task<string> PushAsync(int correlationId, string label, string body)
        {
            using (BrokeredMessage message = new BrokeredMessage(body))
            {
                message.CorrelationId = correlationId.ToString();
                message.Label = label;

                await _queueClient.SendAsync(message);

                return message.MessageId;
            }
        }

        public async Task PushAsync(QueueMessage<T> message)
        {
            var body = await MessageSerializer.Serialize(message.Item);

            using (var brokeredMessage = new BrokeredMessage(body))
            {
                brokeredMessage.CorrelationId = message.CorrelationId.ToString();

                await _queueClient.SendAsync(brokeredMessage);
                message.MessageId = brokeredMessage.MessageId;
            }
        }

        public async Task<QueueMessage<T>> ReceiveAsync()
        {
            var message = await _queueClient.ReceiveAsync();

            if (message == null)
            {
                return null;
            }

            var body = message.GetBody<string>();

            var item = await MessageSerializer.Deserialize(body);

            var queueMessage = new QueueMessage<T>
            {
                MessageId = message.MessageId,
                Item = await MessageSerializer.Deserialize(body)
            };

            try
            {
                queueMessage.CorrelationId = int.Parse(message.CorrelationId);
            }
            catch { }

            return queueMessage;

        }

        public async Task<QueueMessage<T>> ReceiveAsync(TimeSpan timeout)
        {
            if (timeout != TimeSpan.Zero)
            {
                var message = await _queueClient.ReceiveAsync(timeout);

                if (message == null)
                {
                    return null;
                }

                var body = message.GetBody<string>();

                var queueMessage = new QueueMessage<T>
                {
                    MessageId = message.MessageId,
                    Item = await MessageSerializer.Deserialize(body)
                };

                try
                {
                    queueMessage.CorrelationId = int.Parse(message.CorrelationId);
                }
                catch { }

                return queueMessage;
            }
            else
            {
                return await ReceiveAsync();
            }
        }

        public async Task<QueueMessage<T>> PeekAsync()
        {
            BrokeredMessage message = await _queueClient.PeekAsync();

            QueueMessage<T> queueMessage = null;

            if (message != null)
            {
                var body = message.GetBody<string>();

                queueMessage = new QueueMessage<T>
                {
                    MessageId = message.MessageId,
                    Item = await MessageSerializer.Deserialize(body)
                };
            }

            try
            {
                queueMessage.CorrelationId = int.Parse(message.CorrelationId);
            }
            catch { }

            return queueMessage;
        }

        public async Task<QueueMessage<T>> PeekAsync(long sequenceNumber)
        {
            BrokeredMessage message = await _queueClient.PeekAsync(sequenceNumber);

            QueueMessage<T> queueMessage = null;

            if (message != null)
            {
                var body = message.GetBody<string>();

                queueMessage = new QueueMessage<T>
                {
                    MessageId = message.MessageId,
                    Item = await MessageSerializer.Deserialize(body)
                };

                try
                {
                    queueMessage.CorrelationId = int.Parse(message.CorrelationId);
                }
                catch { }
            }

            return queueMessage;
        }

        public void Initializer(string queueName)
        {
            _queueName = queueName;

            _sbConnectionString = CloudConfigurationManager.GetSetting("sbConnectionString");

            CheckCreateQueue(queueName);

            _queueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _queueName, ReceiveMode.ReceiveAndDelete);
        }

        public void Close()
        {
            if (_queueClient != null)
            {
                _queueClient.CloseAsync();
            }
        }

        public string QueueName
        {
            get { return _queueName; }
        }

        public IMessageSerializer<T> MessageSerializer
        {
            get
            {
                return _messageSerializer;
            }

            set
            {
                _messageSerializer = value;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_queueClient != null)
                {
                    _queueClient.Close();
                    _queueClient = null;
                }
            }
        }

        #endregion
    }
}
