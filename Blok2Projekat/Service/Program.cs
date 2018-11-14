using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using UserLogic;
using WCFCommon;

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

            binding.MaxReceivedMessageSize = 5000000;
            binding.MaxBufferSize = 5000000;
            binding.MaxBufferPoolSize = 5000000;

            string address = "net.tcp://localhost:9292/ServiceComms";

            ServiceHost clientCommsHost = new ServiceHost(typeof(ServiceCommsImplementation));

            clientCommsHost.AddServiceEndpoint(typeof(IServiceComms), binding, address);

            // Omogucavamo poboljsani debug mode, kod postoji iskljucivo za debug svrhe 
            ServiceDebugBehavior debug = clientCommsHost.Description.Behaviors.Find<ServiceDebugBehavior>();

            
            if (debug == null)
            {
                clientCommsHost.Description.Behaviors.Add(
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

            //Deo koda koji implementira CustomAuthorizationPolicy i osigurava da se koristi nas CustomPrincipal
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            clientCommsHost.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
            clientCommsHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            clientCommsHost.Open();

            Console.WriteLine("Main service is active.");
            Console.WriteLine("Press <enter> to stop service...");

            Console.ReadLine();
            clientCommsHost.Close();
        }
    }
}