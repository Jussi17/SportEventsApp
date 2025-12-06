using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SportEventsApp.Helpers;
using SportEventsApp.Models;
using System;
using System.Diagnostics;

namespace SportEventsApp;

public partial class AppShell : Shell
{
    private bool _pendingLoginRedirect = false;
    private bool _isLoggingOut = false;
    public static event EventHandler NotificationsChanged;
    public static event EventHandler RoleChanged;

    public AppShell()
    {
        InitializeComponent();
        ValidateLoginStatus();

        Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
        Routing.RegisterRoute("AdminPage", typeof(Pages.AdminPage));
        Routing.RegisterRoute("CalendarPage", typeof(Pages.CalendarPage));
        Routing.RegisterRoute("EventCardPage", typeof(Pages.EventCardPage));
        Routing.RegisterRoute("EventsListPage", typeof(Pages.EventsListPage));
        Routing.RegisterRoute("PalveluPage", typeof(Pages.PalveluPage));
        Routing.RegisterRoute("RegisterPage", typeof(Pages.RegisterPage));

        this.Navigating += AppShell_Navigating;

        // Sulkee hampurilaisvalikon navigoinnin jälkeen
        this.Navigated += (s, e) =>
        {
            FlyoutIsPresented = false;
        };

        UpdateLoginMenuItem();
    }

    private void ValidateLoginStatus()
    {
        try
        {
            bool loggedIn = IsUserLoggedIn();
            string username = Preferences.Get("Username", string.Empty);
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "users.db3");

            if (!File.Exists(dbPath))
            {
                Preferences.Clear();
                return;
            }
            if (loggedIn && !string.IsNullOrEmpty(username))
            {
                var userRepo = new UserRepository();
                var user = userRepo.GetAllUsers().FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    Preferences.Clear();
                }
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine($"ValidateLoginStatus error: {e.Message}");
            Preferences.Clear();
        }
    }

    private async void AppShell_Navigating(object sender, ShellNavigatingEventArgs args)
    {
        // Älä tarkista jos ollaan kirjautumassa ulos
        if (_isLoggingOut)
            return;

        var target = args?.Target?.Location?.OriginalString ?? string.Empty;
        if (!string.IsNullOrEmpty(target) &&
            target.IndexOf("AdminPage", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            bool loggedIn = Preferences.Get("IsLoggedIn", false);
            bool isAdmin = UserRoleHelper.IsAdmin;
            System.Diagnostics.Debug.WriteLine($"Navigating to AdminPage - LoggedIn: {loggedIn}, IsAdmin: {isAdmin}");

            if (!loggedIn)
            {
                // Ei kirjautunut -> vain ohjaa LoginPagelle
                args.Cancel();
                await Shell.Current.Navigation.PushModalAsync(new Pages.LoginPage());  // Change to modal
            }
            else if (!isAdmin)
            {
                // Kirjautunut mutta ei admin -> näytä ilmoitus
                args.Cancel();
                await Shell.Current.DisplayAlert("Pääsy evätty",
                    "Sinulla ei ole oikeuksia tälle sivulle. Tarvitset admin-käyttäjän.",
                    "OK");
            }
        }
    }

    public static void RaiseRoleChanged()
    {
        RoleChanged?.Invoke(null, EventArgs.Empty);
    }

    private bool IsUserLoggedIn() => Preferences.Get("IsLoggedIn", false);

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool loggedIn = IsUserLoggedIn();
        if (!loggedIn)
        {
            await Shell.Current.Navigation.PushModalAsync(new Pages.LoginPage());  // Change to modal
        }
        else
        {
            // Tarkista missä ollaan
            var current = Shell.Current?.CurrentState?.Location?.OriginalString ?? string.Empty;
            bool isOnAdminPage = current.IndexOf("AdminPage", StringComparison.OrdinalIgnoreCase) >= 0;

            _isLoggingOut = true;
            Preferences.Set("IsLoggedIn", false);
            Preferences.Set("Role", "user");
            Preferences.Remove("Username");
            UpdateLoginMenuItem();
            RaiseRoleChanged();

            // Jos ollaan AdminPagella, mene pois sieltä
            if (isOnAdminPage)
            {
                await Shell.Current.GoToAsync("//EventsListPage");
            }
            _isLoggingOut = false;
        }
    }

    public void UpdateLoginMenuItem()
    {
        var username = Preferences.Get("Username", "");
        if (LoginMenuItem == null) return;
        LoginMenuItem.Text = IsUserLoggedIn() ? $"Kirjaudu ulos ({username})" : "Kirjaudu sisään";
    }

    private void TeemaToggled(object sender, ToggledEventArgs e)
    {
        bool isDarkMode = e.Value;
        Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        Preferences.Set("AppTheme", isDarkMode ? "Dark" : "Light");
        if (TeemaSwitch != null)
        {
            TeemaSwitch.OnColor = isDarkMode ? Colors.Blue : Colors.White;
            TeemaSwitch.ThumbColor = isDarkMode ? Colors.White : Colors.Blue;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LoadThemePreference();
        LoadNotificationPreference();
        UpdateLoginMenuItem();

        if (_pendingLoginRedirect)
        {
            _pendingLoginRedirect = false;
            await Task.Delay(50);
            await Shell.Current.GoToAsync("/LoginPage");
        }
    }

    private void LoadThemePreference()
    {
        string savedTheme = Preferences.Get("AppTheme", "Light");
        bool isDarkMode = savedTheme == "Dark";
        Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        if (TeemaSwitch != null)
            TeemaSwitch.IsToggled = isDarkMode;
    }

    private void LoadNotificationPreference()
    {
        bool enabled = Preferences.Get("NotificationsEnabled", false);

        if (NotificationSwitch != null)
        {
            NotificationSwitch.IsToggled = enabled;
            NotificationSwitch.OnColor = enabled ? Colors.Blue : Colors.White;
            NotificationSwitch.ThumbColor = enabled ? Colors.White : Colors.Blue;
        }
    }

    private void OnNotificationToggled(object sender, ToggledEventArgs e)
    {
        bool enabled = e.Value;
        Preferences.Set("NotificationsEnabled", enabled);
        NotificationsChanged?.Invoke(this, EventArgs.Empty);

        if (NotificationSwitch != null)
        {
            NotificationSwitch.OnColor = enabled ? Colors.Blue : Colors.White;
            NotificationSwitch.ThumbColor = enabled ? Colors.White : Colors.Blue;
        }
    }
}