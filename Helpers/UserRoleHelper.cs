using Microsoft.Maui.Storage;

namespace SportEventsApp.Helpers
{
    public static class UserRoleHelper
    {
        public static bool IsAdmin
        {
            get
            {
                var role = Preferences.Get("Role", "user");
                var isLoggedIn = Preferences.Get("IsLoggedIn", false);

                System.Diagnostics.Debug.WriteLine($"UserRoleHelper - IsLoggedIn: {isLoggedIn}, Role: {role}");

                return isLoggedIn && role.Equals("admin", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
