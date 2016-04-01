using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Queue.Provider.Samples.Console.Message
{
    public class SampleMessage
    {
        #region Public Properties

        public int SampleMessageId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; internal set; }

        #endregion

        public override string ToString()
        {
            return string.Format("SampleMessageId: {0} - Name: {1} - Created: {2}", SampleMessageId, Name, Created);
        }

    }
}
