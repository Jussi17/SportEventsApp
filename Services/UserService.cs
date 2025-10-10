using SportEventsApp.Models;

namespace SportEventsApp.Services
{
    public static class UserService
    {
        private static List<User> users = new()
        {
            new User { Username = "admin", Password = "admin123", Role = "admin" },
            new User { Username = "user", Password = "user123", Role = "user" }
        };

        public static User? Login(string username, string password)
        {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public static bool Register(string username, string password)
        {
            if (users.Any(u => u.Username == username))
                return false;
            users.Add(new User { Username = username, Password = password, Role = "user" }); 
            return true;
        }
    }
}
