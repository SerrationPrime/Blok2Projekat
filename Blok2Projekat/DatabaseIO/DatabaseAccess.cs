using DatabaseIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseIO
{
    //Format primljenih zahteva za pisanje/izmenu: SID:X;Timestamp:X;Details:X;
    //Pri upisu, dodaje se na pocetak ID:X;


    public class DatabaseAccess
    {
        public static int currentId;
        private string fName;
        /// <summary>
        /// Konstruktor DatabaseAccess, kreira database i eventLog ako je potrebno.
        /// </summary>
        /// <param name="fileName">Naziv .txt fajla sa kojim zelimo da radimo.</param>
        public DatabaseAccess(string fileName)
        {
            fName = fileName;
            if (!File.Exists(fileName))
            {
                var myFile = File.Create(fileName);
                currentId = 0;
                //Ako se ovo ne uradi, dolazi do problema usled zauzeca fajla
                myFile.Close();
                if (!EventLog.SourceExists("MySource"))
                {
                    EventLog.CreateEventSource("MySource", "MyNewLog");
                }
            }
            else
            {
                //long sanChk = new FileInfo(fileName).Length;
                //utvrdjuje se trenutni ID u bazi podataka
                if (new FileInfo(fileName).Length != 0)
                {
                    while (true)
                    {
                        try
                        {
                            currentId = Int32.Parse(File.ReadLines(fileName).Last().Split(';')[0].Split(':')[1]);
                            break;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(500);
                        }
                    }
                    currentId++;
                }
                else
                    currentId = 0;
                if (!EventLog.SourceExists("MySource"))
                {
                    EventLog.CreateEventSource("MySource", "MyNewLog");
                }
            }
        }
        /// <summary>
        /// Iscitavanje zeljenog dela baze podataka.
        /// </summary>
        /// <param name="sid">SID podataka koje zelimo da citamo. Ako je prazan, cita se citava baza podataka.</param>
        /// <returns>Zeljeni deo baze.</returns>
        public string Read(string sid)
        {
            string returnedValue = string.Empty;
            string[] redString;
            EventLog readingLog = new EventLog();
            readingLog.Source = "MySource";
            string readString = "";
            string[] redString2;

            readingLog.WriteEntry("User requests using Read function.\n");

            if (String.IsNullOrEmpty(sid))
            {
                readingLog.WriteEntry("User sent a request to read all information from the database.\n");
                while (true)
                {
                    try
                    {
                        using (TextReader tr = new StreamReader(fName))
                        {
                            string line;
                            // Read and display lines from the file until the end of 
                            // the file is reached.
                            while ((line = tr.ReadLine()) != null)
                            {
                                returnedValue += line;
                                returnedValue += Environment.NewLine;
                            }
                            tr.Close();
                            readingLog.WriteEntry("All users have been read.");
                            break;
                        }
                    }
                    catch (IOException ex)
                    {
                        Thread.Sleep(500);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("There was an error: " + e.Message);
                        returnedValue = "Error";
                        break;
                    }
                }
            }
            else
            {
                readingLog.WriteEntry("User sent a request to read one events from the database.\n");
                while (true)
                {
                    try
                    {
                        using (TextReader tr = new StreamReader(fName))
                        {
                            string line;
                            // Read and display lines from the file until the end of 
                            // the file is reached.
                            while ((line = tr.ReadLine()) != null)
                            {
                                redString = line.Split(';');
                                redString2 = redString[1].Split(':');

                                if (sid.Equals(redString2[1]))
                                {
                                    returnedValue += line;
                                    returnedValue += Environment.NewLine;
                                    readString = "Successfully read an event with Server Id:" + sid + ".\n";
                                    readingLog.WriteEntry(readString);
                                }
                            }
                            tr.Close();
                            if (returnedValue.Equals(string.Empty))
                            {
                                readString = "User requested to read an event with Server Id:" + sid + ", but there was no such event.\n";
                                readingLog.WriteEntry(readString);
                                returnedValue = ("There is no event with this id");
                            }
                            break;
                        }
                    }
                    catch (IOException ex)
                    {
                        Thread.Sleep(500);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("There was an error: " + e.Message);
                        returnedValue = "Error";
                        break;
                    }
                }
            }

            return returnedValue;
        }
        /// <summary>
        /// Dodavanje nove linije podataka u bazi podataka na koju je DatabaseAccess inicijalizovan. Ovde je potrebno uraditi generaciju id-a.
        /// </summary>
        /// <param name="data">Novi podatak koji treba upisati. Formatiranje podatka je odgovornost pozivaoca. Na kraj podatka treba dodati \n.</param>
        /// <returns>True u slucaju pravilnog upisa, inace false.</returns>
        public bool Write(string data)
        {
            bool written = false;
            string writtenString = string.Empty;
            EventLog writingLog = new EventLog();
            writingLog.Source = "MySource";
            string writeString = "";

            if (!CheckData(data))
            {
                return false;
            }


            writingLog.WriteEntry("User requested Write function");
            while (true)
            {
                try
                {
                    using (TextWriter tw = new StreamWriter(fName, true))
                    {
                        writtenString = "ID:" + currentId + ";" + data;

                        tw.WriteLine(writtenString);
                        currentId++;

                        written = true;
                        writeString = "Data:" + data + "with an Id: " + (currentId - 1) + " Successfully written in the database";
                        writingLog.WriteEntry(writeString);

                        tw.Close();

                        break;
                    }
                }
                catch (IOException ex)
                {
                    Thread.Sleep(500);
                }
                catch (Exception e)
                {

                    Console.WriteLine("There was an error: " + e.Message);
                    written = false;
                    break;
                }
            }
            return written;
        }

        /// <summary>
        /// Izmena linije podataka u bazi podataka na koju je DatabaseAccess inicijalizovan.
        /// </summary>
        /// <param name="id">Identifikator reda koji treba izmeniti.</param>
        /// <param name="data">Novi podatak koji treba upisati. Formatiranje podatka je odgovornost pozivaoca. Na kraj podatka treba dodati \n.</param>
        /// <returns>True u slucaju pravilnog upisa, inace false.</returns>
        public bool Edit(string id, string data)
        {
            bool changed = false;
            string fileContent = File.ReadAllText(fName);
            string[] l;
            string[] l2;
            EventLog editingLog = new EventLog();
            editingLog.Source = "MySource";
            string editString = "";

            string[] lines = fileContent.Split('\n');

            string lineToChange = "";
            editingLog.WriteEntry("User requested Edit function");

            foreach (string line in lines)
            {
                l = line.Split(';');
                l2 = l[0].Split(':');
                if (id.Equals(l2[1]))
                {
                    lineToChange = line;
                    changed = true;
                    editingLog.WriteEntry("Data for Requested Id found");
                    break;
                }
                else
                {
                    changed = false;
                }
            }

            if (changed == true)
            {
                fileContent = fileContent.Replace(lineToChange, "ID:" + id + ";" + data);
                while (true)
                {
                    try
                    {
                        using (TextWriter w = new StreamWriter(fName))
                        {
                            w.Write(fileContent);

                            w.Close();
                        }

                        editString = "Data changed for an event in the database with Id: " + id;
                        editingLog.WriteEntry(editString);
                        break;
                    }
                    catch (IOException ex)
                    {
                        Thread.Sleep(500);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("There was an error: " + e.Message);
                        changed = false;
                        break;
                    }
                }
            }
            else
            {
                editingLog.WriteEntry("Data not changed because the event with the requested id doesn't exist");
            }

            return changed;
        }
        /// <summary>
        /// Brisanje linije podataka u bazi podataka na koju je DatabaseAccess inicijalizovan.
        /// </summary>
        /// <param name="id">Identifikator reda koji treba izbrisati.</param>
        /// <returns>True u slucaju uspesnog brisanja, inace false.</returns>
        public bool Delete(string id)
        {
            bool deleted = false;

            string fileContent = File.ReadAllText(fName);
            string[] l;
            string[] l2;
            EventLog deletingLog = new EventLog();
            deletingLog.Source = "MySource";
            string deleteString = "";


            string[] lines = fileContent.Split('\n');

            string lineToChange = "";

            deletingLog.WriteEntry("User requested Delete function");
            foreach (string line in lines)
            {
                l = line.Split(';');
                l2 = l[0].Split(':');
                if (l2.Length == 2)
                {
                    if (id.Equals(l2[1]))
                    {
                        lineToChange = line;
                        deleted = true;
                        deletingLog.WriteEntry("Data for Requested Id found");
                        break;
                    }
                    else
                    {
                        deleted = false;
                    }
                }
            }

            if (deleted == true)
            {
                fileContent = fileContent.Replace(lineToChange + "\n", string.Empty);
                while (true)
                {
                    try
                    {
                        using (TextWriter w = new StreamWriter(fName))
                        {
                            w.Write(fileContent);

                            w.Close();
                        }
                        break;
                    }
                    catch (IOException ex)
                    {
                        Thread.Sleep(500);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("There was an error: " + e.Message);
                        deleted = false;
                        break;
                    }
                }
                deleteString = "Data deleted for an event in the database with Id: " + id;
                deletingLog.WriteEntry(deleteString);
            }
            else
            {
                deletingLog.WriteEntry("Data not deleted because the event with the requested id doesn't exist");
            }

            return deleted;
        }

        public bool HasRightToModify(string id, string sid)
        {
            bool result = false;

            using (StreamReader sr = new StreamReader(File.OpenRead(fName)))
            {
                string currentId = String.Empty;
                string line = String.Empty;
                string[] parts = { };

                while ((line = sr.ReadLine()) != null && !currentId.Equals("ID:" + id))
                {
                    parts = line.Split(';');
                    // id ; sid ; timestemp ; detail 
                    // 0     1        2         3
                    currentId = parts[0];
                }
                if (parts.Count() > 0 && currentId==("ID:"+id))
                {
                    string CurrentSID = parts[1];
                    if (CurrentSID.Equals("SID:" + sid))
                        result = true;
                }
                else
                {
                    Console.WriteLine("The database does not contain the requested element.");
                }
            }

            return result;
        }

        public bool CheckData(string data)
        {
            bool chked = false; //Nije checked jer je checked rezervisana rec
            string[] chkString;

            chkString = data.Split(';');

            if (chkString.Length == 4)
            {
                Regex r1 = new Regex(@"SID:[A-Za-z0-9_'-'\s\p{P}]");
                Regex r2 = new Regex(@"Timestamp:[A-Za-z0-9_'-'':'\s\p{P}]");
                Regex r3 = new Regex(@"Details:[A-Za-z0-9_\s\p{P}]");

                if (r1.Match(chkString[0]).Success && r2.Match(chkString[1]).Success && r3.Match(chkString[2]).Success)
                {
                    chked = true;
                }
            }
            else
            {
                Console.WriteLine("Nepravilno poslat podatak.");
                Console.WriteLine("Podatak mora biti formata SID:X;Timestamp:Y;Details:Z");
            }

            return chked;
        }
    }
}
