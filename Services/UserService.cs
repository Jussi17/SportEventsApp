using SportEventsApp.Models;
using Microsoft.Maui.Controls; //Application.Current
using Microsoft.Extensions.DependencyInjection; //GetService
using System.Diagnostics; 
namespace SportEventsApp.Services
{
    public static class UserService
    {
        private static UserRepository userRepo;
        static UserService()
        {
            //Vähän voodoota, koska luokka on staattinen
            userRepo = Application.Current?.Handler?.MauiContext?.Services?.GetService<UserRepository>();

            if (userRepo == null)
            {
                Debug.WriteLine("UserRepo on null (UserService)");
            }
        }

        public static User? Login(string username, string password)
        {
            if (userRepo == null) return null;
            return userRepo.GetAllUsers().FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public static bool Register(string username, string password)
        {
            if (userRepo == null) return false;

            if (userRepo.GetAllUsers().Any(u => u.Username == username))
                return false;
            userRepo.InsertUser(new User { Username = username, Password = password, Role = "user" }); 
            return true;
        }
    }
}
