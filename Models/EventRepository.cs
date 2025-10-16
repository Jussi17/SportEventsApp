using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEventsApp.Models;

public class EventRepository
{
    private SQLiteConnection db;

    public EventRepository()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "events.db3");
        //Tietokannan sijainti: System.Diagnostics.Debug.WriteLine($"DB Path: {dbPath}");
        db = new SQLiteConnection(dbPath);
        db.CreateTable<Event>();

        if (!db.Table<Event>().Any())
        {
            var defaultEvents = new List<Event>
            {
                // Jalkapallo
                new Event { Sport = "Jalkapallo", Name = "Veikkausliiga: HJK - KuPS", Date = new DateTime(2025, 7, 18, 18, 30, 0), Location = "Helsinki", Channel = "Ruutu+", Description = "Jännittävä ottelu, jossa pelaavat kaksi Veikkausliigan huippujoukkuetta." },
                new Event { Sport = "Jalkapallo", Name = "Mestarien liiga - Finaali", Date = new DateTime(2026, 5, 30, 21, 0, 0), Location = "Lontoo", Channel = "Yle Areena", Description = "Seurajoukkuekauden huipentuma, legendaarinen Mestarien Liigan finaali." },

                // Jääkiekko
                new Event { Sport = "Jääkiekko", Name = "MM-kisat: Suomi - Ruotsi", Date = new DateTime(2025, 5, 15, 20, 0, 0), Location = "Praha", Channel = "MTV3", Description = "Klassikko-ottelu. Leijonat kohtaa arkkivihollisensa Tre Kronorin." },
                new Event { Sport = "Jääkiekko", Name = "NHL Stanley Cup - Finaali 7. peli", Date = new DateTime(2026, 6, 12, 2, 0, 0), Location = "Denver", Channel = "Viaplay", Description = "Mahdollinen ottelu. Ratkaiseva sarjan viimeinen ottelu." },

                // Koripallo
                new Event { Sport = "Koripallo", Name = "NBA Finaali - Game 1", Date = new DateTime(2026, 6, 5, 4, 0, 0), Location = "Los Angeles", Channel = "Prime Video", Description = "Maailman parhaan koripallosarjan finaalisarja käynnistyy." },
                new Event { Sport = "Koripallo", Name = "Susijengi - Espanja", Date = new DateTime(2025, 11, 12, 19, 0, 0), Location = "Helsinki", Channel = "Yle", Description = "EM-kisoissa upeasti pelanneen Susijengin ottelu huippumaata Espanjaa vastaan." },

                // Amerikkalainen jalkapallo
                new Event { Sport = "Amerikkalainen jalkapallo", Name = "Super Bowl LX", Date = new DateTime(2026, 2, 8, 1, 30, 0), Location = "Las Vegas", Channel = "Nelonen", Description = "Amerikkalaisen jalkapallon huikaiseva tapahtuma pelataan jo 60:nen kerran." },

                // Golf
                new Event { Sport = "Golf", Name = "The Masters", Date = new DateTime(2026, 4, 9, 15, 0, 0), Location = "Augusta", Channel = "Eurosport", Description = "Se kaikista suurin ja arvostetuin Golf-turnaus, jonka jokainen pelaaja unelmoi voittavansa." },
                new Event { Sport = "Golf", Name = "Ryder Cup", Date = new DateTime(2027, 9, 24, 10, 0, 0), Location = "Rooma", Channel = "V Sport Golf", Description = "Pystyykö USA katkaisemaan Euroopan voittoputken Italiassa pelattavassa kilpailussa." },

                // Yleisurheilu
                new Event { Sport = "Yleisurheilu", Name = "MM-kisat 100m finaali", Date = new DateTime(2025, 8, 23, 21, 0, 0), Location = "Tokio", Channel = "Yle", Description = "Kuka onkaan maailman nopein ihminen?" },
                new Event { Sport = "Yleisurheilu", Name = "Olympialaiset: Keihään finaali", Date = new DateTime(2028, 7, 28, 19, 0, 0), Location = "Los Angeles", Channel = "Discovery+", Description = "Suomen jokavuotinen mitalitoivo, joko vihdoin aukeaa Suomen mitalitili?" },

                // Formula 1
                new Event { Sport = "Formula 1", Name = "Monacon GP", Date = new DateTime(2025, 5, 25, 16, 0, 0), Location = "Monte Carlo", Channel = "Viaplay", Description = "Monacon ahtailla kaduilla kilpaillaan jälleen upeissa maisemissa." },
                new Event { Sport = "Formula 1", Name = "Suomen GP", Date = new DateTime(2027, 7, 18, 16, 0, 0), Location = "KymiRing", Channel = "MTV3", Description = "Ensimmäistä kertaa Suomessa kilpaillaan Formula 1-luokassa." },

                // Tennis
                new Event { Sport = "Tennis", Name = "Wimbledon Finaali", Date = new DateTime(2026, 7, 12, 15, 0, 0), Location = "Lontoo", Channel = "Eurosport", Description = "Kaikkien tennisturnausten kuningas." },
                new Event { Sport = "Tennis", Name = "US Open - Miesten finaali", Date = new DateTime(2025, 9, 7, 23, 0, 0), Location = "New York", Channel = "Eurosport", Description = "Kauden viimeinen Grand Slam-turnauksen loppuottelu." },

                // Muita lajeja
                new Event { Sport = "Pyöräily", Name = "Tour de France - Loppu", Date = new DateTime(2025, 7, 27, 18, 0, 0), Location = "Pariisi", Channel = "Eurosport", Description = "Legendaarisen pyöräilykilpailun ratkaiseva etappi." },
                new Event { Sport = "Uinti", Name = "Olympialaiset: 200m vapaauinti finaali", Date = new DateTime(2028, 7, 25, 17, 0, 0), Location = "Los Angeles", Channel = "Yle", Description = "Kenen kunto ja nopeus on tällä kertaa ylivertainen?" },
                new Event { Sport = "Mäkihyppy", Name = "Mäkiviikko", Date = new DateTime(2026, 1, 6, 19, 0, 0), Location = "Bischofshofen", Channel = "Eurosport", Description = "Mäkiviikon päätöskilpailu." }
            };
            db.InsertAll(defaultEvents);
        }
    }

    public List<Event> GetAllEvents() => db.Table<Event>().ToList();

    public List<Event> GetUpcomingEvents() =>
        db.Table<Event>().Where(e => e.Date >= DateTime.Now).ToList();

    public List<Event> GetPastEvents() =>
        db.Table<Event>().Where(e => e.Date < DateTime.Now).ToList();

    public void InsertEvent(Event evt) => db.Insert(evt);
    public void UpdateEvent(Event evt) => db.Update(evt);
    public void DeleteEvent(Event evt) => db.Delete(evt);
}