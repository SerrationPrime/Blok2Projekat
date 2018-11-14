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
            string address = "net.tcp://localhost:9292/ServiceComms";

            InstanceContext instanceContext = new InstanceContext(new ServiceCallback());

            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address)), instanceContext))
            {
                int code;
                string id;
                string text = "";

                
                th.Start();

                Console.ReadLine();

                while (true)
                {
                    code = -1;
                    Console.WriteLine("1. Read()");
                    Console.WriteLine("2. Modify()");
                    Console.WriteLine("3. Subscribe()");
                    Console.WriteLine("0. EXIT");
                    Console.WriteLine("\nEnter the operation code:");

                    int op = Int32.Parse(Console.ReadLine());

                    if (op == 0)
                        break;

                    switch (op)
                    {
                        case 0:
                            Console.WriteLine("Exit the application...");
                            break;
                        case 1:
                            Console.WriteLine("Trying to access the database...\n");
                            Console.Write(proxy.Read());
                            break;
                        case 2:
                            Console.WriteLine("1. Edit");
                            Console.WriteLine("2. Delete");
                            code = Int32.Parse(Console.ReadLine());
                            if (code == 1)
                            {
                                Console.WriteLine("Enter the ID data that changes:");
                                id = Console.ReadLine();
                                Console.WriteLine("Enter new text:");
                                text = Console.ReadLine();
                                string message = "";
                                message += "SID:" + System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.User.ToString() + ";Timestamp:" + DateTime.Now.ToString() + ";Details:" + text + ";";
                                proxy.Modify(ModifyType.Edit, id, message);
                            }
                            else if (code == 2)
                            {
                                Console.WriteLine("Enter the ID data that delete:");
                                id = Console.ReadLine();
                                proxy.Modify(ModifyType.Delete, id, text);
                            }
                            else
                                Console.WriteLine("You entered a non-existent code.");
                            break;
                        case 3:
                            proxy.Subscribe();
                            break;
                        default:
                            Console.WriteLine("You entered a non-existent operation.");
                            break;
                    }
                }

                Console.WriteLine("Press <enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
