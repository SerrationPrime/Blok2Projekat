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

            //Za rad sa velikim bazama podataka
            binding.MaxReceivedMessageSize = 5000000;
            binding.MaxBufferSize = 5000000;
            binding.MaxBufferPoolSize = 5000000;

            InstanceContext instanceContext = new InstanceContext(new ServiceCallback());

            using (WCFClient proxy = new WCFClient(binding, new EndpointAddress(new Uri(address)), instanceContext))
            {
                int code;
                string id;
                string text = "";
                int op = -1;

                
                th.Start();

                while (op!=0)
                {
                    code = -1;
                    Console.WriteLine("1. Read()");
                    Console.WriteLine("2. Modify()");
                    Console.WriteLine("3. Subscribe()");
                    Console.WriteLine("0. EXIT");
                    Console.WriteLine("\nEnter the operation code:");

                    if (!Int32.TryParse(Console.ReadLine(), out op))
                        op = -1;

                    switch (op)
                    {
                        case 0:
                            Console.WriteLine("Exitting the application...");
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
                                Console.WriteLine("Enter the ID of the entry to change:");
                                id = Console.ReadLine();

                                bool validInput = false;

                                while (!validInput)
                                {
                                    Console.WriteLine("Enter new event data (text cannot contains colons or semicolons):");
                                    text = Console.ReadLine();

                                    if (text.Contains(":") || text.Contains(";"))
                                    {
                                        Console.WriteLine("This data contains a colon or a semicolon. Please try again.");
                                    }
                                    else validInput = true;
                                }

                                string message = "SID:" + System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString() + ";Timestamp:" + DateTime.Now.ToString() + ";Details:" + text + ";";
                                proxy.Modify(ModifyType.Edit, id, message);
                            }
                            else if (code == 2)
                            {
                                Console.WriteLine("Enter the ID of the entry to delete:");
                                id = Console.ReadLine();
                                text = "SID:" + System.Security.Principal.WindowsIdentity.GetCurrent().User.ToString();
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
