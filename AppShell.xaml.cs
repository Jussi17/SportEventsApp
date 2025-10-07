using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;

namespace SportEventsApp;

public partial class AppShell : Shell
{
    private bool _pendingLoginRedirect = false;
    public static event EventHandler NotificationsChanged;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
        Routing.RegisterRoute("AdminPage", typeof(Pages.AdminPage));
        Routing.RegisterRoute("CalendarPage", typeof(Pages.CalendarPage));
        Routing.RegisterRoute("EventCardPage", typeof(Pages.EventCardPage));
        Routing.RegisterRoute("EventsListPage", typeof(Pages.EventsListPage));
        Routing.RegisterRoute("PalveluPage", typeof(Pages.PalveluPage));
        Routing.RegisterRoute("RegisterPage", typeof(Pages.RegisterPage));

        this.Navigating += AppShell_Navigating;

        UpdateLoginMenuItem();
    }

    private void AppShell_Navigating(object sender, ShellNavigatingEventArgs args)
    {
        var target = args?.Target?.Location?.OriginalString ?? string.Empty;

        if (!string.IsNullOrEmpty(target) &&
            target.IndexOf("AdminPage", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            bool loggedIn = Preferences.Get("IsLoggedIn", false);
            if (!loggedIn)
            {
                args.Cancel();
                _pendingLoginRedirect = true;
            }
        }
    }

    private bool IsUserLoggedIn() => Preferences.Get("IsLoggedIn", false);

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool loggedIn = IsUserLoggedIn();

        if (!loggedIn)
        {
            await Shell.Current.GoToAsync("/LoginPage");
        }
        else
        {
            Preferences.Set("IsLoggedIn", false);
            UpdateLoginMenuItem();

            var current = Shell.Current?.CurrentState?.Location?.OriginalString ?? string.Empty;
            if (!string.IsNullOrEmpty(current) &&
                current.IndexOf("AdminPage", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                await Shell.Current.GoToAsync("///"); // Absoluuttinen reitti etusivulle
            }
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
            await Task.Delay(50); // Varmistaa, että Shell on valmis
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
