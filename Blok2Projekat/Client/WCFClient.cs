using System;
using System.ServiceModel;
using WCFCommon;

namespace Client
{
    public class WCFClient : DuplexChannelFactory<IServiceComms>, IServiceComms, IDisposable
    {
        IServiceComms factory;

        public WCFClient(NetTcpBinding binding, EndpointAddress address, InstanceContext instanceContext)
            : base(instanceContext, binding, address)
        {
            factory = this.CreateChannel();
        }

        public bool Event(string generatedEvent)
        {
            bool check = false;
            try
            {
                check = factory.Event(generatedEvent);
                //Event je uvek dozvoljen
                //if(provera)
                    //Console.WriteLine("Event() allowed.");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while trying to Event(). {0}", e.Message);
            }
            return check;
        }

        public bool Modify(ModifyType type, string id, string newVersion)
        {
            bool check = false;
            try
            {
                check = factory.Modify(type, id, newVersion);
                if (check)
                    Console.WriteLine("Modify() allowed.");
                else
                    Console.WriteLine("Modify() was not allowed. Either the user cannot modify that element, or the database has no entry with a matching ID.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Modify(). {0}", e.Message);
            }
            return check;
        }

        public string Read()
        {
            string check = "";
            try
            {
                check = factory.Read();
                if (check != "Permission error.")
                    Console.WriteLine("Read() allowed.");
                else
                    Console.WriteLine("Read() not allowed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Read(). {0}", e.Message);
            }
            if (String.IsNullOrEmpty(check))
            {
                Console.WriteLine("There are no entries in the database accessible with the current privilege level.");
            }
            else if (check == "Permission error.")
            {
                check = "";
            }
            return check;
        }

        public bool Subscribe()
        {
            bool check = false;
            try
            {
                check = factory.Subscribe();
                if (check)
                    Console.WriteLine("Subscribe() allowed.");
                else
                    Console.WriteLine("Subscribe() not allowed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe(). {0}", e.Message);
            }
            return check;
        }
    }
}
