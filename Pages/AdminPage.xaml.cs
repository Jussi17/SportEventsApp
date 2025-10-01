using Microsoft.Maui.Controls;
using SportEventsApp.Models;
using System;
using System.Collections.ObjectModel;

namespace SportEventsApp.Pages
{
    public partial class AdminPage : ContentPage
    {
        private ObservableCollection<Event> AdminEvents;

        public AdminPage()
        {
            InitializeComponent();
            if (!Preferences.Get("IsLoggedIn", false))
            {
                Shell.Current.GoToAsync("LoginPage");
            }
            AdminEvents = [];
            AdminEventsCollection.ItemsSource = AdminEvents;
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Set("IsLoggedIn", false);
            Shell.Current.GoToAsync("///");
        }

        private void OnAddEventClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                string.IsNullOrWhiteSpace(LocationEntry.Text) ||
                string.IsNullOrWhiteSpace(ChannelEntry.Text))
            {
                DisplayAlert("Virhe", "Täytä kaikki kentät!", "OK");
                return;
            }

            var newEvent = new Event
            {
                Name = NameEntry.Text,
                Location = LocationEntry.Text,
                Date = DatePicker.Date.Add(TimePicker.Time),
                Channel = ChannelEntry.Text
            };

            AdminEvents.Add(newEvent);

            NameEntry.Text = "";
            LocationEntry.Text = "";
            ChannelEntry.Text = "";
        }
    }
}
