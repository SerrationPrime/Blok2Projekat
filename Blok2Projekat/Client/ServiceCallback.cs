using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;

namespace Client
{
    public class ServiceCallback : IServiceCallback
    {
        public void PublishChanges(string eventDescription)
        {
            Console.WriteLine("Database change: " + eventDescription);
        }
    }
}
