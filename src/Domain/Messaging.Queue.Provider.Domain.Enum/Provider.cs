using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Enum
{
    public enum Provider
    {
        /// <summary>
        /// Windows azure service bus
        /// </summary>
        ServiceBus = 0,

        /// <summary>
        /// Microsoft message queuing
        /// </summary>
        Msmq = 1
    }
}
