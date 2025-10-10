using Microsoft.Maui.Storage;

namespace SportEventsApp.Helpers
{
    public static class UserRoleHelper
    {
        public static bool IsAdmin => Preferences.Get("Role", "user") == "admin";
    }
}
