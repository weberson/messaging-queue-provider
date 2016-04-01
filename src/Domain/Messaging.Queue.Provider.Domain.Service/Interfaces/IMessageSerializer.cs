using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Domain.Service.Interfaces
{
    public interface IMessageSerializer<T> where T : class
    {
        Task<string> Serialize(T t);

        Task<T> Deserialize(string message);
    }
}
