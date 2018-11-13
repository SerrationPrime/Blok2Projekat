using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
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
            bool provera = false;
            try
            {
                provera = factory.Event(generatedEvent);
                //Event je uvek dozvoljen
                //if(provera)
                    //Console.WriteLine("Event() allowed.");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error while trying to Event(). {0}", e.Message);
            }
            return provera;
        }

        public bool Modify(ModifyType type, string id, string newVersion)
        {
            bool provera = false;
            try
            {
                provera = factory.Modify(type, id, newVersion);
                if (provera)
                    Console.WriteLine("Modify() allowed.");
                else
                    Console.WriteLine("Modify() not allowed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Modify(). {0}", e.Message);
            }
            return provera;
        }

        public string Read()
        {
            string provera = "";
            try
            {
                provera = factory.Read();
                if (!String.IsNullOrEmpty(provera))
                    Console.WriteLine("Read() allowed.");
                else
                    Console.WriteLine("Read() not allowed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Read(). {0}", e.Message);
            }
            return provera;
        }

        public bool Subscribe()
        {
            bool provera = false;
            try
            {
                provera = factory.Subscribe();
                if (provera)
                    Console.WriteLine("Subscribe() allowed.");
                else
                    Console.WriteLine("Subscribe() not allowed.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while trying to Subscribe(). {0}", e.Message);
            }
            return provera;
        }
    }
}
