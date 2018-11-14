using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFCommon
{
    /// <summary>
    /// Interfejs za komunikaciju izmedju servisa i LoadBalancera
    /// </summary>
    [ServiceContract]
    public interface ILoadBalanceComms
    {
        [OperationContract]
        bool Modify(ModifyType type, string id, string newVersion);
        /*
        VAZNO
        Modify samo vraca da je klijent ima dozvolu da vrsi trazenu operaciju.
        Sam upis u bazu se radi asinhrono unutar LoadBalancera, i pretpostavlja se da je uspesan (sto se moze proveriti preko Windows Event loga).
        Potvrda uspesnog upisa bi dodala znatnu kolicinu kompleksnosti usled potrebe za callback funkcijom, pracenja klijentskih zahteva, i vracanja vrednosti pravilnim klijentima.
        Iz tog razloga, ova funkcionalnost nije implementirana.
         */
    }
}