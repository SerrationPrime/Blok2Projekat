using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

public enum ModifyType { Edit, Delete };

namespace WCFCommon
{
    //Format poruke za upis/modifikaciju koja se salje sa klijenta: Timestamp:X;Details:X;
    [ServiceContract]
    public interface IServiceComms
    {
        /// <summary>
        /// Read koji se poziva iz klijenta. SID se NE prosledjuje; utvrdjuje se na strani servisa.
        /// </summary>
        /// <returns>String koji sadrzi svaki red vezan za datog klijenta.</returns>
        [OperationContract]
        string Read();
        /// <summary>
        /// Zahtev za modifikaciju baze podataka sa klijentske strane.
        /// </summary>
        /// <param name="type">Tip modifikacije.</param>
        /// <param name="id">ID podatka koji treba modifikovati.</param>
        /// <param name="newVersion">Zeljeni novi sadrzaj tog podatka. Formatiranje se radi na strani klijenta.</param>
        /// <returns></returns>
        [OperationContract]
        bool Modify(ModifyType type, string id, string newVersion);
        /// <summary>
        /// Zahtev za prijavu na promene baze podataka. Ispisuje svaku novu izmenu na bazi podataka na konzolu posle uspesne konekcije.
        /// </summary>
        /// <returns>Vraca true u slucaju uspesnog zahteva, inace false. Ako se petlja ispisa promena na bazi podataka implementira kao zasebna nit, ne zaboravi da implementiras
        /// graceful shutdown.</returns>
        [OperationContract]
        bool Subscribe();
        /// <summary>
        /// Slanje dogadjaja na servis. U realnom sistemu, ovo bi bila neka druga funkcija koja u svom okviru generise dogadjaje, ali ovde je
        /// predvidjeno da se salje jedan Event() na svaku sekundu, generisan iz resursnog fajla.
        /// </summary>
        /// <param name="generatedEvent">Nasumice generisan Event() iz resursnog fajla</param>
        /// <returns>Vraca true ako je dogadjaj uspesno upisan, inace false.</returns>
        [OperationContract]
        bool Event(string generatedEvent);
    }
}
