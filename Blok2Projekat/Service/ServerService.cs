using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;

namespace Service
{
    public class ServerService : IServiceComms
    {
        public bool Event(string generatedEvent)
        {
            throw new NotImplementedException();
        }

        public bool Modify(ModifyType type, string id, string newVersion)
        {
            throw new NotImplementedException();
        }

        public string Read()
        {
            throw new NotImplementedException();
        }

        public bool Subscribe()
        {
            throw new NotImplementedException();
        }
    }
}
