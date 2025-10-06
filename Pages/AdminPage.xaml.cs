using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SportEventsApp.Models;
using System;
using System.Linq;

namespace SportEventsApp.Pages;

public partial class AdminPage : ContentPage
{
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
            Id = EventsListPage.Events.Any() ? EventsListPage.Events.Max(ev => ev.Id) + 1 : 1,
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
        SportEntry.Text = "";
        NameEntry.Text = "";
        LocationEntry.Text = "";
        DatePicker.Date = DateTime.Now;
        TimePicker.Time = TimeSpan.Zero;
        ChannelEntry.Text = "";
        DescriptionEntry.Text = "";
    }
}
