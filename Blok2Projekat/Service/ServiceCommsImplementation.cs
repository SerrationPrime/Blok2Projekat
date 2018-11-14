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
        DatabaseAccess accessPoint = new DatabaseAccess("../../../Database.txt");
        ILoadBalanceComms proxy;

        public delegate void DBChangeEventHandler(object sender, string eventDescription);
        /// <summary>
        /// Skup svih prosledjenih delegata, pozivanje ovoga pozivamo dogadjaje na svim subscribovanim klijentima
        /// </summary>
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
            catch (Exception e)
            {
                throw e;
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
                catch (Exception e)
                {
                    throw e;
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
            try
            {
                if (HasPermission(Permission.Supervise))
                {
                    return accessPoint.Read("");
                }
                else if (HasPermission(Permission.Read))
                {
                    return accessPoint.Read(GetSid());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Permission error.";
        }

        public bool Subscribe()
        {
            if (HasPermission(Permission.Subscribe))
            {
                //Prvo, pokupimo objekat koji se odnosi na klijenta koji poziva ovu funkciju
                ServiceCallback = OperationContext.Current.GetCallbackChannel<IServiceCallback>();
                //Zatim, kreiramo delegat na funkciju CallbackInvoker, koji poziva funkciju koja svim subscribovanim korisnicima ispisuje podatke o promeni
                DBEventHandler = new DBChangeEventHandler(CallbackInvoker);
                //...i dodajemo je u DBChangeEvent, sto je skup svih delegata koje smo kreirali, i koji ih poziva svaki put kada se desi promena; vidi pozive DBChangeEvent
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

        /// <summary>
        /// Funkcija ciji se delegati kreiraju, samo poziva PublishChanges iz callback-a
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventDescription">Opis dogadjaja.</param>
        public void CallbackInvoker(object sender, string eventDescription)
        {
            ServiceCallback.PublishChanges(eventDescription);
        }
    }
}
