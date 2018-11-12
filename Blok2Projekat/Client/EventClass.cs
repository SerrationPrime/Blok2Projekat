using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class EventClass
    {
        public void generateEvent()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9292/ServiceComms";

            InstanceContext instanceContext = new InstanceContext(new ServiceCallback());

            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address)), instanceContext))
            {
                int kod = -1;
                while(true)
                {
                    Random rnd = new Random();

                    kod = rnd.Next(10);

                    switch(kod)
                    {
                        case 0:
                            proxy.Event("A");
                            break;
                        case 1:
                            proxy.Event("B");
                            break;
                        case 2:
                            proxy.Event("C");
                            break;
                        case 3:
                            proxy.Event("D");
                            break;
                        case 4:
                            proxy.Event("E");
                            break;
                        case 5:
                            proxy.Event("F");
                            break;
                        case 6:
                            proxy.Event("G");
                            break;
                        case 7:
                            proxy.Event("H");
                            break;
                        case 8:
                            proxy.Event("I");
                            break;
                        case 9:
                            proxy.Event("J");
                            break;
                        default:
                            proxy.Event("O");
                            break;
                    }

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
