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
            //this.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            factory = this.CreateChannel();
        }
        public bool Modify(ModifyType type, string id, string newVersion)
        {
            bool result = false;
            try
            {
                string sid = System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.User.ToString();
                result = factory.Modify(type, id, newVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Modify] ERROR = {0}", e.Message);
                throw e;
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