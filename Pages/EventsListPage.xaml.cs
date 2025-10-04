using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using SportEventsApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Graphics;

#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
#endif

namespace SportEventsApp.Pages;

public partial class EventsListPage : ContentPage
{
    public static ObservableCollection<Event> Events { get; set; }
    public ObservableCollection<Event> FilteredEvents { get; set; }

    private bool showUpcoming = true; // Tulevat oletuksena

    public EventsListPage()
    {
        InitializeComponent();

        Events = new ObservableCollection<Event>
        {
     // Jalkapallo
    new Event { Id = 1, Sport = "Jalkapallo", Name = "Veikkausliiga: HJK - KuPS", Date = new DateTime(2025, 7, 18, 18, 30, 0), Location = "Helsinki", Channel = "Ruutu+" },
    new Event { Id = 2, Sport = "Jalkapallo", Name = "Mestarien liiga - Finaali", Date = new DateTime(2026, 5, 30, 21, 0, 0), Location = "Lontoo", Channel = "Yle Areena" },

    // Jääkiekko
    new Event { Id = 3, Sport = "Jääkiekko", Name = "MM-kisat: Suomi - Ruotsi", Date = new DateTime(2025, 5, 15, 20, 0, 0), Location = "Praha", Channel = "MTV3" },
    new Event { Id = 4, Sport = "Jääkiekko", Name = "NHL Stanley Cup - Finaali 7. peli", Date = new DateTime(2026, 6, 12, 2, 0, 0), Location = "Denver", Channel = "Viaplay" },

    // Koripallo
    new Event { Id = 5, Sport = "Koripallo", Name = "NBA Finaali - Game 1", Date = new DateTime(2026, 6, 5, 4, 0, 0), Location = "Los Angeles", Channel = "Prime Video" },
    new Event { Id = 6, Sport = "Koripallo", Name = "Susijengi - Espanja", Date = new DateTime(2025, 11, 12, 19, 0, 0), Location = "Helsinki", Channel = "Yle" },

    // Amerikkalainen jalkapallo
    new Event { Id = 7, Sport = "Amerikkalainen jalkapallo", Name = "Super Bowl LX", Date = new DateTime(2026, 2, 8, 1, 30, 0), Location = "Las Vegas", Channel = "Nelonen" },

    // Golf
    new Event { Id = 8, Sport = "Golf", Name = "The Masters", Date = new DateTime(2026, 4, 9, 15, 0, 0), Location = "Augusta", Channel = "Eurosport" },
    new Event { Id = 9, Sport = "Golf", Name = "Ryder Cup", Date = new DateTime(2027, 9, 24, 10, 0, 0), Location = "Rooma", Channel = "V Sport Golf" },

    // Yleisurheilu
    new Event { Id = 10, Sport = "Yleisurheilu", Name = "MM-kisat 100m finaali", Date = new DateTime(2025, 8, 23, 21, 0, 0), Location = "Tokio", Channel = "Yle" },
    new Event { Id = 11, Sport = "Yleisurheilu", Name = "Olympialaiset: Keihään finaali", Date = new DateTime(2028, 7, 28, 19, 0, 0), Location = "Los Angeles", Channel = "Discovery+" },

    // Formula 1
    new Event { Id = 12, Sport = "Formula 1", Name = "Monacon GP", Date = new DateTime(2025, 5, 25, 16, 0, 0), Location = "Monte Carlo", Channel = "Viaplay" },
    new Event { Id = 13, Sport = "Formula 1", Name = "Suomen GP", Date = new DateTime(2027, 7, 18, 16, 0, 0), Location = "KymiRing", Channel = "MTV3" },

    // Tennis
    new Event { Id = 14, Sport = "Tennis", Name = "Wimbledon Finaali", Date = new DateTime(2026, 7, 12, 15, 0, 0), Location = "Lontoo", Channel = "Eurosport" },
    new Event { Id = 15, Sport = "Tennis", Name = "US Open - Miesten finaali", Date = new DateTime(2025, 9, 7, 23, 0, 0), Location = "New York", Channel = "Eurosport" },

    // Muita lajeja
    new Event { Id = 16, Sport = "Pyöräily", Name = "Tour de France - Loppu", Date = new DateTime(2025, 7, 27, 18, 0, 0), Location = "Pariisi", Channel = "Eurosport" },
    new Event { Id = 17, Sport = "Uinti", Name = "Olympialaiset: 200m vapaauinti finaali", Date = new DateTime(2028, 7, 25, 17, 0, 0), Location = "Los Angeles", Channel = "Yle" },
    new Event { Id = 18, Sport = "Mäkihyppy", Name = "Mäkiviikko", Date = new DateTime(2026, 1, 6, 19, 0, 0), Location = "Bischofshofen", Channel = "Eurosport" }
        };

        // Lajit vasemmalle
        var sports = Events.Select(e => e.Sport).Distinct().ToList();
        sports.Insert(0, "Kaikki lajit");
        SportsList.ItemsSource = sports;

        // FilteredEvents
        FilteredEvents = new ObservableCollection<Event>();
        BindingContext = this;

        UpdateFilteredEvents("Kaikki lajit");

        // Ladataan tallennetut Notify-arvot
        foreach (var e in Events)
        {
            e.Notify = Preferences.Get($"Notify_{e.Id}", false);
            // Näytä kellonappi vain tuleville tapahtumille
            e.NotifyVisible = e.Date >= DateTime.Now && Preferences.Get("NotificationsEnabled", true);
        }

        BindingContext = this;
    }

    public static ObservableCollection<Event> GetAllEvents()
    {
        return Events;
    }

    private async void OnEventTapped(object sender, EventArgs e)
    {
        if (sender is Grid grid && grid.BindingContext is Event evt)
        {
            await Navigation.PushAsync(new EventCardPage(evt));
        }
    }

    private void OnSportSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not string selectedSport) return;
        UpdateFilteredEvents(selectedSport);
    }

    private void OnUpcomingClicked(object sender, EventArgs e)
    {
        showUpcoming = true;
        UpcomingButton.BackgroundColor = Colors.Blue;
        UpcomingButton.TextColor = Colors.White;
        PastButton.TextColor = Colors.Black;
        PastButton.BackgroundColor = Colors.LightGray;
        UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
    }

    private void OnPastClicked(object sender, EventArgs e)
    {
        showUpcoming = false;
        UpcomingButton.BackgroundColor = Colors.LightGray;
        UpcomingButton.TextColor = Colors.Black;
        PastButton.TextColor = Colors.White;
        PastButton.BackgroundColor = Colors.Blue;
        UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
    }

    private void UpdateFilteredEvents(string selectedSport)
    {
        FilteredEvents.Clear();
        var query = Events.Where(ev => showUpcoming ? ev.Date >= DateTime.Now : ev.Date < DateTime.Now);

        if (!string.IsNullOrEmpty(selectedSport) && selectedSport != "Kaikki lajit")
            query = query.Where(ev => ev.Sport == selectedSport);

        foreach (var ev in query.OrderBy(ev => ev.Date))
            FilteredEvents.Add(ev);
    }

    private void OnClockClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Event evt)
        {
            evt.Notify = !evt.Notify;
            Preferences.Set($"Notify_{evt.Id}", evt.Notify);

            if (evt.Notify)
            {
                var notifyTime = evt.Date;
                DisplayAlert("Ilmoitus ajastettu", $"Saat ilmoituksen tapahtumasta {evt.Name}, {evt.Date}", "OK");
                ScheduleNotification(evt);
            }
            else
                CancelNotification(evt);
        }
    }

    private void ScheduleNotification(Event tapahtuma)
    {
        bool notificationsEnabled = Preferences.Get("NotificationsEnabled", true);
        if (!notificationsEnabled || !tapahtuma.Notify)
            return;

#if WINDOWS
        // Windows Toast
        ShowWindowsNotification(
            "Ottelu tulossa!",
            $"{tapahtuma.Name} alkaa {tapahtuma.Date:dd.MM.yyyy HH:mm}"
        );
#else
        // Plugin.LocalNotification
        var notification = new NotificationRequest
        {
            NotificationId = tapahtuma.Id,
            Title = "Ottelu tulossa!",
            Description = $"{tapahtuma.Name} alkaa {tapahtuma.Date:dd.MM.yyyy HH:mm}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(5)
            }
        };
        LocalNotificationCenter.Current.Show(notification);
#endif
    }

#if WINDOWS
    private void ShowWindowsNotification(string title, string message)
    {
        var toastContent = new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .GetToastContent();

        var toast = new ToastNotification(toastContent.GetXml());
        ToastNotificationManager.CreateToastNotifier("SportEventsApp").Show(toast);
    }
#endif

    private void CancelNotification(Event evt)
    {
        LocalNotificationCenter.Current.Cancel(evt.Id);
        DisplayAlert("Ilmoitus poistettu", $"Et saa ilmoitusta tapahtumasta {evt.Name}, {evt.Date}", "OK");
    }

    // Lajilistan hover-efekti
    private void OnSportPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Grid grid && grid.Parent is Frame frame)
        {
            frame.Scale = 1.05;
            frame.BackgroundColor = Color.FromArgb("#0066CC"); // Tummempi sininen
        }
    }

    private void OnSportPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Grid grid && grid.Parent is Frame frame)
        {
            frame.Scale = 1.0;
            frame.BackgroundColor = Colors.Blue;
        }
    }

    // Tapahtumien hover-efekti
    private void OnEventPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Grid grid && grid.Parent is Frame frame)
        {
            frame.Scale = 1.02;
            frame.Opacity = 0.9;
        }
    }

    private void OnEventPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Grid grid && grid.Parent is Frame frame)
        {
            frame.Scale = 1.0;
            frame.Opacity = 1.0;
        }
    }
}
