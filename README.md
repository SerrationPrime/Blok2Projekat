# Blok2Projekat, zadatak 24

Implementirati servis za upravljanje bazom podataka koja sadrži spisak događaja izgenerisanih od strane klijenata (koji sa servisnom komponentom komuniciraju preko Windows autentifikacionog protokola). Spisak validnih događaja je unapred definisan u okviru resursnog fajla, dok se u bazu podataka (tekstualni fajl) uz odgovarajuću poruku o detaljima događaja skladište sledeće informacije:

* jedinstveni identifikator entiteta u bazi podataka,
* Security Identifier (SID) i korisničko ime klijenta koji je izgenerisao događaj,
* Timestamp kad je dati događaj izgenerisan.

Servis omogućuje sledeće akcije nad bazom podataka:

* Ukoliko korisnik ima *Read* privilegiju, može da pročita događaje koje je on izgenerisao.
* Ukoliko korisnik ima *Supervise* privilegiju može da pročita kompletan sadržaj baze.
* Za ažuriranje sadržaja baze (modifikacija ili brisanje postojećih entiteta), pšroverava se privilegija Modify i zatim zahtev preusmerava na LoadBalancer komponentu, koja na osnovu SID-a (koji se ne sme proslediti kao parametar prilikom poziva) dodatno proverava da li korisnik pokučava da izmeni entitet čiji je vlasnik, i u zavisnosti od toga izvršava zahtevanu operaciju ili ne.
* Validne zahteve za ažuriranje sadržaja baze LoadBalancer prosleđuje slobodnoj Worker komponenti sa najmanjim cost faktorom u tom trenutku.

Druga grupa klijenata (*Subscribe* permisija) su klijenti koji ispisuju kompletan sadržaj iz baze na konzoli, tako da je uvek ispisano najnovije stanje u bazi podataka (nakon svake izmenme u bazi podataka, servis obaveštava pretplaćene klijente o update-u).

Dodatno, sve pokušaje uspešnog i neuspešnog pristupa bazi je potrebno logovati u specifičnom Windows Event Logu.
