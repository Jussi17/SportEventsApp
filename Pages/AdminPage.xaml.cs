using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SportEventsApp.Models;
using System;
using System.Data;
using System.Linq;

namespace SportEventsApp.Pages;

public partial class AdminPage : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (!Preferences.Get("IsLoggedIn", true))
        {
            Shell.Current.GoToAsync("//LoginPage");
            return;  
        }

        var username = Preferences.Get("Username", "");
        var role = Preferences.Get("Role", "user");

        UsernameLabel.Text = $"Kirjautunut: {username} ({role})";

        bool isAdmin = role == "admin";
        FormContainer.IsVisible = isAdmin;
        NoPermissionLabel.IsVisible = !isAdmin;
    }
    public AdminPage()
    {
        InitializeComponent();

        // Pakotetaan 24h-kello
        TimePicker.Format = "HH:mm";

        // Kirjautumisen tarkistus
        if (!Preferences.Get("IsLoggedIn", false))
        {
            Shell.Current.GoToAsync("/LoginPage");
        }
    }

    private void OnAddEventClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(LocationEntry.Text) ||
            string.IsNullOrWhiteSpace(ChannelEntry.Text) ||
            string.IsNullOrWhiteSpace(SportEntry.Text))
        {
            DisplayAlert("Virhe", "T‰yt‰ kaikki kent‰t!", "OK");
            return;
        }

        var newEvent = new Event
        {
            Sport = SportEntry.Text,
            Name = NameEntry.Text,
            Location = LocationEntry.Text,
            Date = DatePicker.Date.Add(TimePicker.Time),
            Channel = ChannelEntry.Text,
            Description = DescriptionEntry.Text
        };

        // Lis‰‰ tapahtuma EventsListPage:‰‰n ja p‰ivit‰ FilteredEvents
        if (EventsListPage.CurrentInstance != null)
        {
            EventsListPage.CurrentInstance.AddAndRefreshEvent(newEvent);
        }

        // Tyhjennet‰‰n kent‰t
        DisplayAlert("Onnistui", "Tapahtuma lis‰tty!", "OK");
        SportEntry.Text = "";
        NameEntry.Text = "";
        LocationEntry.Text = "";
        DatePicker.Date = DateTime.Now;
        TimePicker.Time = TimeSpan.Zero;
        ChannelEntry.Text = "";
        DescriptionEntry.Text = "";
    }
}
