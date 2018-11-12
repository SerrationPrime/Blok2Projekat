using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;

namespace Service
{
    //U sustini, service ce samo da poziva metode definisane u DataIO klasi.
    //Jedina razlika jeste u tome sto ce pri pozivu Modify() - istog pozvati impersonirajuci klijenta. 

    public class SeverService : IServiceComms
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
