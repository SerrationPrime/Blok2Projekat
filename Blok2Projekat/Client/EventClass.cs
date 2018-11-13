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

                    string eventToSend;

                    switch(kod)
                    {
                        case 0:
                            eventToSend= "A";
                            break;
                        case 1:
                            eventToSend = "B";
                            break;
                        case 2:
                            eventToSend = "C";
                            break;
                        case 3:
                            eventToSend = "D";
                            break;
                        case 4:
                            eventToSend = "E";
                            break;
                        case 5:
                            eventToSend = "F";
                            break;
                        case 6:
                            eventToSend = "G";
                            break;
                        case 7:
                            eventToSend = "H";
                            break;
                        case 8:
                            eventToSend = "I";
                            break;
                        case 9:
                            eventToSend = "J";
                            break;
                        default:
                            eventToSend = "O";
                            break;
                    }

                    proxy.Event("Timestamp:" + DateTime.Now.ToString() + ";Details:" + eventToSend + ";");

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
