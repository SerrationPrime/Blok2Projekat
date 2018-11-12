using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            EventClass EC = new EventClass();

            Thread th = new Thread(new ThreadStart(EC.generateEvent));


            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            string address = "net.tcp://localhost:9999/ServerService";

            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address))))
            {
                int kod = -1;
                string id;
                string text = "";

                
                th.Start();

                while (true)
                {
                    Console.WriteLine("1. Read()");
                    Console.WriteLine("2. Modify()");
                    Console.WriteLine("3. Subscribe()");
                    Console.WriteLine("0. EXIT");
                    Console.WriteLine("\nUnesite kod operacije:");

                    int operacija = Int32.Parse(Console.ReadLine());

                    if (operacija == 0)
                        break;

                    switch (operacija)
                    {
                        case 0:
                            Console.WriteLine("Izlaz iz aplikacije...");
                            break;
                        case 1:
                            proxy.Read();
                            break;
                        case 2:
                            Console.WriteLine("1. Edit");
                            Console.WriteLine("2. Delete");
                            kod = Int32.Parse(Console.ReadLine());
                            if (kod == 1)
                            {
                                Console.WriteLine("Unesite ID podatka koji editujete:");
                                id = Console.ReadLine();
                                Console.WriteLine("Unesite novi text:");
                                text = Console.ReadLine();
                                string poruka = "";
                                poruka += "Timestamp:" + DateTime.Now.ToString() + ";Details:" + text + ";";
                                proxy.Modify(ModifyType.Edit, id, poruka);
                            }
                            else if (kod == 2)
                            {
                                Console.WriteLine("Unesite ID podatka koji se brise:");
                                id = Console.ReadLine();
                                proxy.Modify(ModifyType.Delete, id, text);
                            }
                            else
                                Console.WriteLine("Uneli ste nepostojeci kod.");
                            break;
                        case 3:
                            proxy.Subscribe();
                            break;
                        default:
                            Console.WriteLine("Uneli ste nepostojecu operaciju.");
                            break;
                    }
                }

                Console.WriteLine("Pritisni enter za izlaz.");
                Console.ReadLine();
            }
        }
    }
}
