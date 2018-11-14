using DatabaseIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCFCommon;

namespace LoadBalancer
{
    //TODO: na pocetku rada LB-a inicijalizuj sve liste na pocetne vrednosti (ili pri koriscenju, ako nije prazna, nekase napravi.)
    public struct Argument
    {
        public ModifyType type;
        public string id;
        public string data;

        public Argument(ModifyType modifyType, string id1, string data1)
        {
            type = modifyType;
            id = id1;
            data = data1;
        }

        public override string ToString()
        {
            return type.ToString() + " " + id + " " + data;
        }
    };

    public class LoadBalancerServices : ILoadBalanceComms
    {
        const int _maxNbOfTasks = 4;
  
        public static Thread[] tasks = new Thread[_maxNbOfTasks];
        public static bool[] retVals = new bool[_maxNbOfTasks];
        public static Argument[] arguments = new Argument[_maxNbOfTasks];
        public static bool[] isBusy = new bool[_maxNbOfTasks];
        public static bool[] requestToProcess = new bool[_maxNbOfTasks];

        public static int currentIdx = 0;

        private DatabaseAccess _dbAccess = new DatabaseAccess("../../../Database.txt");

        public LoadBalancerServices()
        {
            for (int i = 0; i < _maxNbOfTasks; i++)
            {
                int currVal = i;
                tasks[i] = new Thread(() => Worker(currVal));

                retVals[i] = false;
                requestToProcess[i] = false;
                isBusy[i] = true;

                tasks[i].Start();
                isBusy[i] = false;
            }
        }

        /// <summary>
        /// Stara se o obradi korisnickog zahteva. Proverava njegova prava na pristup i salje zahtev ka odgovarajucem workeru na obradu.
        /// </summary>
        /// <param name="type">tTip izmene koji se zahteva</param>
        /// <param name="id">Id reda u bazi, nad kojim se vrsi izmena</param>
        /// <param name="newVersion">Podatak za upis u bazu</param>
        /// <returns>True ako je izmena uspesna. Inace false.</returns>
        //[OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public bool Modify(ModifyType type, string id, string newVersion)
        {
            //get sid from newWersion data.
            string[] lines = newVersion.Split(';');     //newVersion: SID:xxx;Timestamp:xxx;Details:xxx;
            string[] sidColumn = lines[0].Split(':');   //SID xxx
            string sid = sidColumn[1];                  //xxx

            Console.WriteLine("Modify called on LB. ");
            bool canModify = _dbAccess.HasRightToModify(id, sid);

            if (canModify)
            {
                ChooseWorkerAndSend(type, id, newVersion);
            }
            return canModify;
        }
        /// <summary>
        /// Nalazi slobodnog workera sa najmanjim costID i prosledjuje mu podatke za obradu.
        /// Cost id workera je definisan njegovim indeksom u listi. Worker sa manjim indeksom ima manjin costID
        /// </summary>
        /// <param name="type">Tip modifikacije po kome worker poziva odgovarajucu akciju nad bazom</param>
        /// <param name="id">Red u bazi koji se menja</param>
        /// <param name="data">Novi podaci za bazu</param>
        /// <param name="retVal">Povratna vrednost koja se odnosi na uspesnost workerove radnje</param>
        public void ChooseWorkerAndSend(ModifyType type, string id, string data)
        {
            bool found = false;
            Argument argToWorkWith = new Argument(type, id, data);
            while (!found)
            {
                //go through all workers; check the one with smalest costID; if busy, search for next one; if not, give him the client data for processing
                int cnt = 0;

                foreach (var item in isBusy)
                {
                    if (item)
                        cnt++;
                    else
                        break;
                }
                if (cnt < _maxNbOfTasks)
                {
                    found = true;
                    arguments[cnt] = argToWorkWith;
                    requestToProcess[cnt] = true;
                }
                else
                    //Svi radnici su zauzeti; cekaj da neki bude oslobodjen
                    Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Proces koji izvrsava radnje zahtevane od klijenta, nakon izvrsene provere o vlasnistvu nad datim podacima.
        /// </summary>
        /// <param name="rbr"> Redni broj pod kojim se isti proces nalazi u listi vrednosti gde je oznaceno da li je slobodan ili ne</param>
        /// <returns></returns>
        public void Worker(int noOfWorker)
        {
            while (Program.ProgramActive)
            {
                if (requestToProcess[noOfWorker])
                {

                    isBusy[noOfWorker] = true;
                        if (arguments[noOfWorker].type == ModifyType.Delete)
                        {
                            retVals[noOfWorker] = _dbAccess.Delete(arguments[noOfWorker].id);
                        }
                        else
                        {
                            retVals[noOfWorker] = _dbAccess.Edit(arguments[noOfWorker].id, arguments[noOfWorker].data);
                        }
                    
                    isBusy[noOfWorker] = false;
                    requestToProcess[noOfWorker] = false;
                }
                else Thread.Sleep(1000);
            }

    }
}
}