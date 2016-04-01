using Messaging.Queue.Provider.Domain.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messaging.Queue.Provider.Domain.Service;
using System.Messaging;
using Messaging.Queue.Provider.Infrastructure.Extensions;
using Messaging.Queue.Provider.Domain.Service.Interfaces.Providers;

namespace Messaging.Queue.Provider.Infrastructure.Providers
{
    public class Msmq<T> : IMsmq<T> where T : class
    {
        #region Private Fields

        private IMessageSerializer<T> _messageSerializer;

        private System.Messaging.MessageQueue _messageQueue;

        private string _queueName;

        #endregion

        #region Constructor

        public Msmq()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get message queue by queue name.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        protected System.Messaging.MessageQueue GetMessageQueue(string queueName)
        {
            System.Messaging.MessageQueue result = null;

            if (System.Messaging.MessageQueue.Exists(queueName))
            {
                result = new System.Messaging.MessageQueue(queueName, System.Messaging.QueueAccessMode.SendAndReceive);
            }
            else
            {
                result = System.Messaging.MessageQueue.Create(queueName);

                try
                {
                    result.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);
                }
                catch (Exception)
                {
                    result.SetPermissions("Todos", System.Messaging.MessageQueueAccessRights.FullControl);
                }
            }

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_messageQueue != null)
                {
                    _messageQueue.Close();
                }
            }
        }

        #endregion

        #region IMessageQueue Members

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

        public string QueueName
        {
            get { return _queueName; }
        }

        public void Close()
        {
            if (_messageQueue != null)
            {
                _messageQueue.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Initializer(string queueName)
        {
            _messageQueue = GetMessageQueue(string.Format(@".\private$\{0}", queueName));

            _messageQueue.MessageReadPropertyFilter.ClearAll();
            _messageQueue.MessageReadPropertyFilter.AppSpecific = true;
            _messageQueue.MessageReadPropertyFilter.Priority = true;
            _messageQueue.MessageReadPropertyFilter.Label = true;
            _messageQueue.MessageReadPropertyFilter.Id = true;
            _messageQueue.MessageReadPropertyFilter.Body = true;
            _messageQueue.Formatter = new ActiveXMessageFormatter();

            _queueName = queueName;
        }

        public async Task<QueueMessage<T>> PeekAsync()
        {
            using (var message = await _messageQueue.PeekAsync())
            {
                var item = await MessageSerializer.Deserialize((string)message.Body);

                return new QueueMessage<T>
                {
                    MessageId = message.Id,
                    Item = item,
                    CorrelationId = message.AppSpecific
                };
            }
        }

        /// <summary>
        /// Push message default priority (normal)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PushAsync(QueueMessage<T> message)
        {
            await PushAsync(message, MessagePriority.Normal);
        }

        public async Task<string> PushAsync(int correlationId, string body)
        {
            return await PushAsync(correlationId, string.Empty, body);
        }

        public async Task<string> PushAsync(int correlationId, string label, string body)
        {
            return await PushAsync(correlationId, label, body, MessagePriority.Normal);
        }

        public async Task PushAsync(QueueMessage<T> message, MessagePriority priority)
        {
            using (var msg = new System.Messaging.Message())
            {
                msg.AppSpecific = message.CorrelationId;
                msg.Body = await MessageSerializer.Serialize(message.Item);
                msg.Recoverable = true;
                msg.Formatter = new ActiveXMessageFormatter();
                msg.Priority = priority;

                _messageQueue.Send(msg);

                message.MessageId = msg.Id;
            }
        }

        public async Task<string> PushAsync(int correlationId, string body, MessagePriority priority)
        {
            return await PushAsync(correlationId, string.Empty, body, priority);
        }

        public Task<string> PushAsync(int correlationId, string label, string body, MessagePriority priority)
        {
            using (var message = new System.Messaging.Message())
            {
                message.AppSpecific = correlationId;
                message.Label = label;
                message.Body = body;
                message.Recoverable = true;
                message.Priority = priority;

                message.Formatter = new ActiveXMessageFormatter();

                _messageQueue.Send(message);

                return Task.FromResult<string>(message.Id);
            }
        }
        
        public async Task<QueueMessage<T>> ReceiveAsync()
        {
            using (var message = await _messageQueue.ReceiveAsync())
            {
                var item = await MessageSerializer.Deserialize((string)message.Body);

                return new QueueMessage<T>
                {
                    MessageId = message.Id,
                    Item = item,
                    CorrelationId = message.AppSpecific
                };
            }
        }

        public async Task<QueueMessage<T>> ReceiveAsync(TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                return await ReceiveAsync();
            }

            System.Messaging.Message message = null;

            try
            {
                message = await _messageQueue.ReceiveAsync(timeout);
            }
            catch (System.Messaging.MessageQueueException e)
            {
                if (message != null)
                {
                    message.Dispose();
                }

                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    throw new TimeoutException("Timeout expired receiving from queue.");
                }
                else
                {
                    throw;
                }
            }

            using (message)
            {
                var item = await MessageSerializer.Deserialize((string)message.Body);

                var queueMessage = new QueueMessage<T>
                {
                    MessageId = message.Id,
                    Item = item,
                    CorrelationId = message.AppSpecific
                };

                return queueMessage;
            }
        }



        #endregion
    }
}
