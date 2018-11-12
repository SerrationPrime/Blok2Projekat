using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;

namespace Service
{
    public class ServiceLoadBalancerProxy : ChannelFactory<ILoadBalanceComms>, IDisposable, ILoadBalanceComms
    {
        ILoadBalanceComms factory;

        public ServiceLoadBalancerProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            //da omogucim impersonikaciju sa metodom Modify
            // Warning: ako ovako uradim impersonate na LoadBalcer-u ce da impersonira -Service-
            // a ja zelim da impersoniram Client-a koji se obratio Service-u
            //Kada Service bude pozivao ovaj Modify() ===> neka ga pozove kao Client.
            //(u Client-u ce biti omogucena impoersonikacija nad njegovom proxy klasom, a na Service-u ce se omoguciti rad sa )
            this.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            factory = this.CreateChannel();
        }

        [OperationBehavior(Impersonation = ImpersonationOption.Required)]   //vidi da li sam radi impersonate ili je samo ne pusta bez
        public bool Modify(ModifyType type, string id, string newVersion)
        {
            bool result = false;
            try
            {
                //using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
                //{
                result = factory.Modify(type, id, newVersion);
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine("[Modify] ERROR = {0}", e.Message);
            }
            return result;
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }
            this.Close();
        }
    }
}
