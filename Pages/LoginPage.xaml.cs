using SportEventsApp.Services;
using SportEventsApp.Models;

namespace SportEventsApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text?.Trim();

            var user = UserService.Login(username, password);

            if (user != null)
            {
                // Tallennetaan kirjautumistila ja rooli
                Preferences.Set("IsLoggedIn", true);
                Preferences.Set("Username", user.Username);
                Preferences.Set("Role", user.Role);

                // P‰ivitet‰‰n AppShellin login-napin teksti
                (Shell.Current as AppShell)?.UpdateLoginMenuItem();

                // Ohjataan roolin mukaan
                if (user.Role == "admin")
                    await Shell.Current.GoToAsync("/AdminPage");
                else
                    await Shell.Current.GoToAsync("/EventsListPage");
            }
            else
            {
                await DisplayAlert("Virhe", "V‰‰r‰ k‰ytt‰j‰tunnus tai salasana", "OK");
            }
        }

        private async void GoToRegister_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("/RegisterPage");
        }
    }
}
