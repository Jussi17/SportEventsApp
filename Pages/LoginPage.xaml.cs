namespace SportEventsApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (UsernameEntry.Text == "testi" && PasswordEntry.Text == "salasana")
        {
            Preferences.Set("IsLoggedIn", true);
            // P‰ivitet‰‰n AppShellin nappi teksti‰ varten
            (Shell.Current as AppShell)?.UpdateLoginMenuItem();
            await Shell.Current.GoToAsync("///AdminPage");
        }
        else
        {
            await DisplayAlert("Virhe", "V‰‰r‰t tunnukset", "OK");
        }
    }
}
