using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEventsApp.Models;

public class UserRepository
{
    private SQLiteConnection userdb;
    public UserRepository()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");
        //Tietokannan sijainti: System.Diagnostics.Debug.WriteLine($"DB Path: {dbPath}");
        userdb = new SQLiteConnection(dbPath);
        userdb.CreateTable<User>();

        if (!userdb.Table<User>().Any())
        {
            InsertUser(new User { Username = "admin", Password = "admin123", Role = "admin" });
            Debug.WriteLine("Käyttäjien tietokanta on tyhjä. Lisätään admin.");
        }
    }

    public List<User> GetAllUsers() => userdb.Table<User>().ToList();
    public void InsertUser(User user) => userdb.Insert(user);
    public void UpdateUser(User user) => userdb.Update(user);
    public void DeleteUser(User user) => userdb.Delete(user);
}
