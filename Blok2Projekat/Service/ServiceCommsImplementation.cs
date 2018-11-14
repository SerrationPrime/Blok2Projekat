using DatabaseIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using UserLogic;
using WCFCommon;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ServiceCommsImplementation : IServiceComms
    {
        /// <summary>
        /// 
        /// </summary>
        DatabaseAccess accessPoint = new DatabaseAccess("../../../Database.txt");
        ILoadBalanceComms proxy;

        public delegate void DBChangeEventHandler(object sender, string eventDescription);
        public static event DBChangeEventHandler DBChangeEvent;

        IServiceCallback ServiceCallback = null;
        DBChangeEventHandler DBEventHandler = null;

        public ServiceCommsImplementation()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;                                                     //siguran kanal
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;    //digitalno potpisivanje podataka
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9000/LoadBalancerService";

            proxy = new ServiceLoadBalancerProxy(binding, address);

        }

        public bool Event(string generatedEvent)
        {
            string messageToSend = "SID:" + GetSid() + ";" + generatedEvent;

            try
            {
                accessPoint.Write(messageToSend);
            }
            catch
            {
                return false;
            }
            if (DBChangeEvent!=null)
                DBChangeEvent(this, "User with SID " + GetSid() + " added event: " + generatedEvent + ".");
            return true;     
        }
        public bool Modify(ModifyType type, string id, string newVersion)   //newVersion contains SID. Client sent it like that.
        {
            bool result = false;
            if (HasPermission(Permission.Modify))
            {
                try
                {
                    result = proxy.Modify(type, id, newVersion);
                }
                catch
                {
                    return result;
                }

            }
            else return result;
            if (DBChangeEvent != null)
            {
                if (type == ModifyType.Edit)
                {
                    DBChangeEvent(this, "User with SID " + GetSid() + " modified event with ID " + id + " to new value " + newVersion + ".");
                }
                else
                {
                    DBChangeEvent(this, "User with SID " + GetSid() + " deleted event with ID " + id + ".");
                }
            }
            return result;
        }

        public string Read()
        {
            if (HasPermission(Permission.Supervise))
            {
                return accessPoint.Read("");
            }
            else if (HasPermission(Permission.Read))
            {
                return accessPoint.Read(GetSid());
            }
            else return "No read permission.\n";
        }

        public bool Subscribe()
        {
            if (HasPermission(Permission.Subscribe))
            {
                ServiceCallback = OperationContext.Current.GetCallbackChannel<IServiceCallback>();
                DBEventHandler = new DBChangeEventHandler(CallbackInvoker);
                DBChangeEvent += DBEventHandler;
                return true;
            }
            else return false;
        }

        bool HasPermission(Permission perm)
        {
            CustomPrincipal currUser = new CustomPrincipal(ServiceSecurityContext.Current.WindowsIdentity);
            return currUser.HasPermission(perm.ToString());
        }

        string GetSid()
        {
            return System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.User.ToString();
        }

        public void CallbackInvoker(object sender, string eventDescription)
        {
            ServiceCallback.PublishChanges(eventDescription);
        }
    }
}
