using System;
using System.Collections.Generic;
using System.IO;
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

            string[] eventToSend = new string[10];

            using (TextReader tr = new StreamReader("../../../Eventbase.txt"))
            {
                string line;
                int i = 0;
                while ((line = tr.ReadLine()) != null)
                {
                    eventToSend[i] = line;
                    i++;
                }
            }
            try
            {
                using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address)), instanceContext))
                {
                    int code;
                    bool connectionActive = true;
                    while (connectionActive)
                    {
                        code = -1;
                        Random rnd = new Random();

                        code = rnd.Next(10);

                        if (!proxy.Event("Timestamp:" + DateTime.Now.ToString() + ";Details:" + eventToSend[code] + ";"))
                        {
                            connectionActive = false;
                            Console.WriteLine("Stopping event generation.");
                        }
                        else Thread.Sleep(1000);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Event generation stopped due to connection error.");
            }
        }
    }
}
