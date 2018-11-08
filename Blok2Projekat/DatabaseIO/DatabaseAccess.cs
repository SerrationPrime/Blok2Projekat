using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// Proverava da li se u redu odredjenom sa id, u bazi, sid poklapa sa sid prosledjenim od LB 
        /// </summary>
        /// <param name="id">definise red u bazi nad kojim se vrsi provera</param>
        /// <param name="sid">sid korisnika koji zeli da pristupi bazi</param>
        /// <returns></returns>
        public bool HasRightToModify(string id, string sid)
        {
            bool result = false;

            using (StreamReader sr = new StreamReader(File.OpenRead("Database.txt")))
            {
                string currentId = String.Empty;
                string line = String.Empty;
                string[] parts = { };

                while((line = sr.ReadLine()) != null && !currentId.Equals(id))
                {
                    parts = line.Split(';');
                    // id ; sid ; timestemp ; detail 
                    // 0     1        2         3
                    currentId = parts[0];
                }

                string CurrentSID = parts[1];
                if (CurrentSID.Equals(sid))
                    result = true;               
            }

            return result;
        }
    }
}
