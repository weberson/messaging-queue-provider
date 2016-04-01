using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Enum
{
    public enum Serializer
    {
        /// <summary>
        /// JavaScript Object Notation
        /// </summary>
        Json = 0,

        /// <summary>
        /// eXtensible Markup Language
        /// </summary>
        Xml = 1
    }
}
