using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseIO
{
    //Format primljenih zahteva za pisanje/izmenu: SID:X;Timestamp:X;Details:X;
    //Pri upisu, dodaje se na pocetak ID:X;
    public class DatabaseAccess
    {
        /// <summary>
        /// Konstruktor DatabaseAccess.
        /// </summary>
        /// <param name="fileName">Naziv .txt fajla sa kojim zelimo da radimo.</param>
        public DatabaseAccess(string fileName)
        {

        }
        /// <summary>
        /// Iscitavanje zeljenog dela baze podataka.
        /// </summary>
        /// <param name="sid">SID podataka koje zelimo da citamo. Ako je prazan, cita se citava baza podataka.</param>
        /// <returns>Zeljeni deo baze.</returns>
        public string Read(string sid)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Dodavanje nove linije podataka u bazi podataka na koju je DatabaseAccess inicijalizovan. Ovde je potrebno uraditi generaciju id-a.
        /// </summary>
        /// <param name="data">Novi podatak koji treba upisati. Formatiranje podatka je odgovornost pozivaoca. Na kraj podatka treba dodati \n.</param>
        /// <returns>True u slucaju pravilnog upisa, inace false.</returns>
        public bool Write(string data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Izmena linije podataka u bazi podataka na koju je DatabaseAccess inicijalizovan.
        /// </summary>
        /// <param name="id">Identifikator reda koji treba izmeniti.</param>
        /// <param name="data">Novi podatak koji treba upisati. Formatiranje podatka je odgovornost pozivaoca. Na kraj podatka treba dodati \n.</param>
        /// <returns>True u slucaju pravilnog upisa, inace false.</returns>
        public bool Edit(string id, string data)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Brisanje linije podataka u bazi podataka na koju je DatabaseAccess inicijalizovan.
        /// </summary>
        /// <param name="id">Identifikator reda koji treba izbrisati.</param>
        /// <returns>True u slucaju uspesnog brisanja, inace false.</returns>
        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
