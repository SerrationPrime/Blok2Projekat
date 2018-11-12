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
    public class LoadBalancerServices : ILoadBalanceComms
    {
        //List<Task> tasks = new List<Task>();
        List<bool> IsBusy = new List<bool>();   //pravim 4 procesa i 4 bool-a, test
        //dobro je napraviti i listu int-ova za povratne vrednosti. Inicijalno je postavljena na -1. Worker obavi posao i ako je uspesan, postavi vrednost na 1, ako je neuspesan, postavi vrednost na 0. Modify proveri vrednost za datog workera i vrati vrednost na -1, (inace je nije procitao). Vrati odgovazrajucu povratnu vrednost.

        //Ako su svi workeri zauzeti, i klijent posalje zahtev, on nigde nece otici, niti ce se sacuvati
        //znaci da mora postojati neka lista/red izmedju klijenta i korisnika u koji ce se stavljati zahtevi koje service posalje. a LB ce iz njih da cita zahtev i salje ga workeru.

        private DatabaseAccess _dbAccess = new DatabaseAccess("Database.txt");
        
        /// <summary>
        /// Stara se o obradi korisnickog zahteva. Proverava njegova prava na pristup i salje zahtev ka odgovarajucem workeru na obradu.
        /// </summary>
        /// <param name="type">tTip izmene koji se zahteva</param>
        /// <param name="id">Id reda u bazi, nad kojim se vrsi izmena</param>
        /// <param name="newVersion">Podatak za upis u bazu</param>
        /// <returns>True ako je izmena uspesna. Inace false.</returns>
        public bool Modify(ModifyType type, string id, string newVersion)
        {
            //zelim da ga pozivam kao Client, kako bi mogla da proverim njegov sid
            // ako je SID iz tog reda == SID mog klijenta koga impersonifikujem -> IMA privilegiju da uradi modifikaciju.
            // posto ima privilegiju -> pozovi upis/brisanje iz baze  (metode koje su implementirane u DataIO)
            bool result = false;
            //bool retValFromWorker = false;
            //using (ServiceSecurityContext.Current.WindowsIdentity.Impersonate())
            //{
            //    SecurityIdentifier sid = System.ServiceModel.ServiceSecurityContext.Current.WindowsIdentity.User;
            //    string newData = sid.ToString() + ";" + newVersion;
            //    if (_dbAccess.HasRightToModify(id, sid.ToString()))
            //    {
            //        ChooseWorkerAndSend(type, id, newVersion, ref retValFromWorker);
            //    }
            //}
            Console.WriteLine("Modify called on LB. ");
            return result;
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
            //prodji kroz sve workere; proveri onog koji ima najmanji costID; ako je zauzet, trazi sledeceg; ako ne, prosledi mu klijentske podatke
            int cnt = 0;
            int nbOfWorkers = IsBusy.Count();
            if (nbOfWorkers > 0)
            {
                foreach (var item in IsBusy)
                {
                    if (item)
                        cnt++;
                    else
                        break;
                }
                if (cnt != nbOfWorkers)
                    Worker(type, id, data, cnt, ref retVal);
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

        /// <summary>
        /// Proces koji izvrsava radnje zahtevane od klijenta, nakon izvrsene provere o vlasnistvu nad datim podacima.
        /// </summary>
        /// <param name="type"> Tip modifikacije koji korisnik zahteva</param>
        /// <param name="id"> Definise red u bazi nad kojim zelimo da vrsimo izmene</param>
        /// <param name="newData"> Podaci za upis u bazu, LB je dodao na njih klijentov sid</param>
        /// <param name="rbr"> Redni broj pod kojim se isti proces nalazi u listi vrednosti gde je oznaceno da li je slobodan ili ne</param>
        /// <returns></returns>
        public bool Worker(ModifyType type, string id, string newData, int rbr, ref bool retVal)
        {
            while (true)
            {
                if (!IsBusy[rbr])
                {
                    IsBusy[rbr] = true;
                    bool result = false;
                    if (type == ModifyType.Delete)
                    {
                        result = _dbAccess.Delete(id);
                    }
                    else
                    {
                        result = _dbAccess.Edit(id, newData);
                    }
                    IsBusy[rbr] = false;
                    retVal = result;
                }
                Thread.Sleep(200);
            }
        }
    }
}
