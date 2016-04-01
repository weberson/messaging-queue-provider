using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service
{
    public class QueueMessage<T> where T : class
    {
        #region Public properties

        /// <summary>
        /// Identificador único (int) da mensagem.
        /// </summary>
        public int CorrelationId;

        /// <summary>
        /// Identificador unico (guid) da mensagem
        /// </summary>
        public string MessageId;

        /// <summary>
        /// Objeto da mensagem.
        /// </summary>
        public T Item;

        #endregion

        #region Contructor

        public QueueMessage()
        {

        }

        public QueueMessage(T item)
            : this()
        {
            Item = item;
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return string.Format("Item:{0} CorrelationId: {1} MessageId: {2} ", Item, CorrelationId, MessageId);
        }

        #endregion
    }
}
