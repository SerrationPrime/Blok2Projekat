using DatabaseIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;

namespace Service
{
    public class ServiceCommsImplementation : IServiceComms
    {
        DatabaseAccess accessPoint = new DatabaseAccess("Database.txt");
        ILoadBalanceComms proxy;

        public ServiceCommsImplementation()
        {
            //srediti konekciju sa Atininim delom
        }

        public bool Event(string generatedEvent)
        {
            string messageToSend = generatedEvent;
            //TODO:izvuci SID, dopisiu generated event
            try
            {
                accessPoint.Write(messageToSend);
            }
            catch
            {
                return false;
            }
            return true;     
        }

        public bool Modify(ModifyType type, string id, string newVersion)
        {
            if (HasPermission(Permission.Modify))
            {
                try
                {
                    proxy.Modify(type, id, newVersion);
                }
                catch
                {
                    return false;
                }
                
            }
            return false;
        }

        public string Read()
        {
            if (HasPermission(Permission.Read))
            {
                return accessPoint.Read(GetSid());
            }
            else return "No read permission.";
        }

        public bool Subscribe()
        {
            throw new NotImplementedException();
        }

        bool HasPermission(Permission perm)
        {
            return true;
        }

        string GetSid()
        {
            throw new NotImplementedException();
        }
    }
}
