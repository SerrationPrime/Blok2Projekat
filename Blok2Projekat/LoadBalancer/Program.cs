using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using WCFCommon;
namespace LoadBalancer
{
    class Program
    {

        public static bool ProgramActive = true;
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;                                                     //siguran kanal
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;    //digitalno potpisivanje podataka
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9000/LoadBalancerService";

            ServiceHost host = new ServiceHost(typeof(LoadBalancerServices));

            /*ServiceAuthorizationBehavior MyServiceAuthoriationBehavior = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
            MyServiceAuthoriationBehavior.ImpersonateCallerForAllOperations = true;*/
            host.AddServiceEndpoint(typeof(ILoadBalanceComms), binding, address);

            // Omogucavamo poboljsani debug mode, kod postoji iskljucivo za debug svrhe
            ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();

            if (debug == null)
            {
                host.Description.Behaviors.Add(
                     new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                // make sure setting is turned ON
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }

            host.Open();

            Console.WriteLine("LoadBalancerService is started.");
            Console.WriteLine("Press <enter> to stop service...");

            Console.ReadLine();

            ProgramActive = false;
            host.Close();
        }
    }
}
