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
        db = new SQLiteConnection(dbPath);
        db.CreateTable<Event>();
    }

    public List<Event> GetAllEvents() => db.Table<Event>().ToList();

    public List<Event> GetUpcomingEvents() =>
        db.Table<Event>().Where(e => e.Date >= DateTime.Now).ToList();

    public List<Event> GetPastEvents() =>
        db.Table<Event>().Where(e => e.Date < DateTime.Now).ToList();

    public void InsertEvent(Event evt) => db.Insert(evt);
    public void UpdateEvent(Event evt) => db.Update(evt);
}