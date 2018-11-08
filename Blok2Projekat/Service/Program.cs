using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;                                                     //siguran kanal
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;    //digitalno potpisivanje podataka
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9000/LoadBalancerService";

            using (ServiceLoadBalancerProxy proxy = new ServiceLoadBalancerProxy(binding, address))
            {
                //pre poziva modify od Load Balancer-a:
                //izvrsiti proveru o tome da li klijent pripada odgovarajucoj grupi. 
                //ako ne pripada, na zvati LoadBalancer-a

                proxy.Modify(ModifyType.Edit, "testID", "new version of detail...");
                proxy.Modify(ModifyType.Delete, "testID", "why am i even entering this when nothing's gonna be done with it...");

            }

            Console.ReadLine();
        }
    }
}
