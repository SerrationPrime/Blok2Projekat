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
            string address = "net.tcp://localhost:9292/ServiceComms";

            ServiceHost clientCommsHost = new ServiceHost(typeof(ServiceCommsImplementation));
            clientCommsHost.AddServiceEndpoint(typeof(ILoadBalanceComms), binding, address);

            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            clientCommsHost.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
            clientCommsHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

            clientCommsHost.Open();

            Console.WriteLine("LoadBalancerService is started.");
            Console.WriteLine("Press <enter> to stop service...");

            Console.ReadLine();
            clientCommsHost.Close();
        }
    }
}
