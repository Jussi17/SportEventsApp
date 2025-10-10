using SportEventsApp.Services;

namespace SportEventsApp.Pages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void Register_Clicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text?.Trim();
            var password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageLabel.Text = "T�yt� kaikki kent�t.";
                return;
            }

            if (UserService.Register(username, password)) 
            {
                await DisplayAlert("Onnistui", "Rekister�inti onnistui!", "OK");
                await Shell.Current.GoToAsync("/LoginPage");
            }
            else
            {
                MessageLabel.Text = "K�ytt�j�tunnus on jo olemassa.";
            }
        }
    }
}