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
        public static List<Task<bool>> tasks = new List<Task<bool>>();
        public static List<bool> retVals = new List<bool>() { false, false, false, false };
        public static List<Argument> arguments = new List<Argument>();
        public static List<bool> isBusy = new List<bool>() { false, false, false, false };   //pravim 4 procesa i 4 bool-a, test
        public static int currentIdx = 0;
        private int _maxNbOftasks = 4;

        private DatabaseAccess _dbAccess = new DatabaseAccess("../../../Database.txt");

        /// <summary>
        /// Stara se o obradi korisnickog zahteva. Proverava njegova prava na pristup i salje zahtev ka odgovarajucem workeru na obradu.
        /// </summary>
        /// <param name="type">tTip izmene koji se zahteva</param>
        /// <param name="id">Id reda u bazi, nad kojim se vrsi izmena</param>
        /// <param name="newVersion">Podatak za upis u bazu</param>
        /// <returns>True ako je izmena uspesna. Inace false.</returns>
        [OperationBehavior(Impersonation = ImpersonationOption.Required)]
        public bool Modify(ModifyType type, string id, string newVersion)
        {
            bool retValFromWorker = false;
            SecurityIdentifier sid = System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.User;
            string newData = "SID:" + sid.ToString() + ";" + newVersion;
            if (_dbAccess.HasRightToModify(id, sid.ToString()))
            {
                ChooseWorkerAndSend(type, id, newData, ref retValFromWorker);

            }
            Console.WriteLine("Modify called on LB. ");
            //return retValFromWorker;
            return retVals[currentIdx];
        }
        /// <summary>
        /// Nalazi slobodnog workera sa najmanjim costID i prosledjuje mu podatke za obradu.
        /// Cost id workera je definisan njegovim indeksom u listi. Worker sa manjim indeksom ima manjin costID
        /// </summary>
        /// <param name="type">Tip modifikacije po kome worker poziva odgovarajucu akciju nad bazom</param>
        /// <param name="id">Red u bazi koji se menja</param>
        /// <param name="data">Novi podaci za bazu</param>
        /// <param name="retVal">Povratna vrednost koja se odnosi na uspesnost workerove radnje</param>
        public void ChooseWorkerAndSend(ModifyType type, string id, string data, ref bool retVal)
        {
            bool found = false;
            while (!found)
            {
                //prodji kroz sve workere; proveri onog koji ima najmanji costID; ako je zauzet, trazi sledeceg; ako ne, prosledi mu klijentske podatke
                int cnt = 0;
                int nbOfWorkers = isBusy.Count();
                if (nbOfWorkers > 0)
                {
                    foreach (var item in isBusy)
                    {
                        if (item)
                            cnt++;
                        else
                            break;
                    }
                    if (cnt != nbOfWorkers)
                    {
                        found = true;
                        tasks.Add(Task.Run(() => Worker(type, id, data, cnt)));
                        tasks[cnt].Wait();
                        currentIdx = cnt;
                        retVal = retVals[currentIdx];
                    }
                    else
                        //nema slobodnih workera, pa se mora cekati da se neko oslobodi.
                        Thread.Sleep(200);
                }
                else
                {
                    //nema workera uopste - onda cekam da se neki ubaci.
                    Thread.Sleep(200);
                }
            }
        }

        /// <summary>
        /// Proces koji izvrsava radnje zahtevane od klijenta, nakon izvrsene provere o vlasnistvu nad datim podacima.
        /// </summary>
        /// <param name="type"> Tip modifikacije koji korisnik zahteva</param>
        /// <param name="id"> Definise red u bazi nad kojim zelimo da vrsimo izmene</param>
        /// <param name="newData"> Podaci za upis u bazu, LB je dodao na njih klijentov sid</param>
        /// <param name="rbr"> Redni broj pod kojim se isti proces nalazi u listi vrednosti gde je oznaceno da li je slobodan ili ne</param>
        /// <returns></returns>
        public bool Worker(ModifyType type, string id, string newData, int rbr)
        {
            while (true)
            {
                if (!isBusy[rbr])
                {
                    isBusy[rbr] = true;
                    bool result = false;
                    if (type == ModifyType.Delete)
                    {
                        result = _dbAccess.Delete(id);
                    }
                    else
                    {
                        result = _dbAccess.Edit(id, newData);
                    }
                    isBusy[rbr] = false;
                    retVals[rbr] = result;
                    return result;
                }
                Thread.Sleep(200);
            }
        }
    }
}
