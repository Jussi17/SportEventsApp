using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using SportEventsApp.Helpers;
using SportEventsApp.Models;
using SportEventsApp.Popups;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
#endif

namespace SportEventsApp.Pages;

public partial class EventsListPage : ContentPage, INotifyPropertyChanged
{
    public static ObservableCollection<Event> Events { get; set; }
    private ObservableCollection<Event> _filteredEvents;
    public ObservableCollection<Event> FilteredEvents
    {
        get => _filteredEvents;
        set
        {
            if (_filteredEvents == value) return;
            _filteredEvents = value;
            OnPropertyChanged();
        }
    }

    public static EventsListPage CurrentInstance { get; private set; }

    public bool showUpcoming = true;

    public EventsListPage()
    {
        InitializeComponent();
        CurrentInstance = this;

        System.Diagnostics.Debug.WriteLine($"IsLoggedIn: {Preferences.Get("IsLoggedIn", false)}");
        System.Diagnostics.Debug.WriteLine($"Role: {Preferences.Get("Role", "NOT_SET")}");
        System.Diagnostics.Debug.WriteLine($"Username: {Preferences.Get("Username", "NOT_SET")}");

        Events = new ObservableCollection<Event>
        {
            // Jalkapallo
           new Event { Id = 1, Sport = "Jalkapallo", Name = "Veikkausliiga: HJK - KuPS", Date = new DateTime(2025, 7, 18, 18, 30, 0), Location = "Helsinki", Channel = "Ruutu+", Description = "Jännittävä ottelu, jossa pelaavat kaksi Veikkausliigan huippujoukkuetta." },
           new Event { Id = 2, Sport = "Jalkapallo", Name = "Mestarien liiga - Finaali", Date = new DateTime(2026, 5, 30, 21, 0, 0), Location = "Lontoo", Channel = "Yle Areena", Description = "Seurajoukkuekauden huipentuma, legendaarinen Mestarien Liigan finaali." },

           // Jääkiekko
           new Event { Id = 3, Sport = "Jääkiekko", Name = "MM-kisat: Suomi - Ruotsi", Date = new DateTime(2025, 5, 15, 20, 0, 0), Location = "Praha", Channel = "MTV3", Description = "Klassikko-ottelu. Leijonat kohtaa arkkivihollisensa Tre Kronorin." },
           new Event { Id = 4, Sport = "Jääkiekko", Name = "NHL Stanley Cup - Finaali 7. peli", Date = new DateTime(2026, 6, 12, 2, 0, 0), Location = "Denver", Channel = "Viaplay", Description = "Mahdollinen ottelu. Ratkaiseva sarjan viimeinen ottelu." },

           // Koripallo
           new Event { Id = 5, Sport = "Koripallo", Name = "NBA Finaali - Game 1", Date = new DateTime(2026, 6, 5, 4, 0, 0), Location = "Los Angeles", Channel = "Prime Video", Description = "Maailman parhaan koripallosarjan finaalisarja käynnistyy." },
           new Event { Id = 6, Sport = "Koripallo", Name = "Susijengi - Espanja", Date = new DateTime(2025, 11, 12, 19, 0, 0), Location = "Helsinki", Channel = "Yle", Description = "EM-kisoissa upeasti pelanneen Susijengin ottelu huippumaata Espanjaa vastaan." },

           // Amerikkalainen jalkapallo
           new Event { Id = 7, Sport = "Amerikkalainen jalkapallo", Name = "Super Bowl LX", Date = new DateTime(2026, 2, 8, 1, 30, 0), Location = "Las Vegas", Channel = "Nelonen", Description = "Amerikkalaisen jalkapallon huikaiseva tapahtuma pelataan jo 60:nen kerran." },

           // Golf
           new Event { Id = 8, Sport = "Golf", Name = "The Masters", Date = new DateTime(2026, 4, 9, 15, 0, 0), Location = "Augusta", Channel = "Eurosport", Description = "Se kaikista suurin ja arvostetuin Golf-turnaus, jonka jokainen pelaaja unelmoi voittavansa." },
           new Event { Id = 9, Sport = "Golf", Name = "Ryder Cup", Date = new DateTime(2027, 9, 24, 10, 0, 0), Location = "Rooma", Channel = "V Sport Golf", Description = "Pystyykö USA katkaisemaan Euroopan voittoputken Italiassa pelattavassa kilpailussa." },

           // Yleisurheilu
           new Event { Id = 10, Sport = "Yleisurheilu", Name = "MM-kisat 100m finaali", Date = new DateTime(2025, 8, 23, 21, 0, 0), Location = "Tokio", Channel = "Yle", Description = "Kuka onkaan maailman nopein ihminen?" },
           new Event { Id = 11, Sport = "Yleisurheilu", Name = "Olympialaiset: Keihään finaali", Date = new DateTime(2028, 7, 28, 19, 0, 0), Location = "Los Angeles", Channel = "Discovery+", Description = "Suomen jokavuotinen mitalitoivo, joko vihdoin aukeaa Suomen mitalitili?" },

           // Formula 1
           new Event { Id = 12, Sport = "Formula 1", Name = "Monacon GP", Date = new DateTime(2025, 5, 25, 16, 0, 0), Location = "Monte Carlo", Channel = "Viaplay", Description = "Monacon ahtailla kaduilla kilpaillaan jälleen upeissa maisemissa." },
           new Event { Id = 13, Sport = "Formula 1", Name = "Suomen GP", Date = new DateTime(2027, 7, 18, 16, 0, 0), Location = "KymiRing", Channel = "MTV3", Description = "Ensimmäistä kertaa Suomessa kilpaillaan Formula 1-luokassa." },

           // Tennis
           new Event { Id = 14, Sport = "Tennis", Name = "Wimbledon Finaali", Date = new DateTime(2026, 7, 12, 15, 0, 0), Location = "Lontoo", Channel = "Eurosport", Description = "Kaikkien tennisturnausten kuningas." },
           new Event { Id = 15, Sport = "Tennis", Name = "US Open - Miesten finaali", Date = new DateTime(2025, 9, 7, 23, 0, 0), Location = "New York", Channel = "Eurosport", Description = "Kauden viimeinen Grand Slam-turnauksen loppuottelu." },

           // Muita lajeja
           new Event { Id = 16, Sport = "Pyöräily", Name = "Tour de France - Loppu", Date = new DateTime(2025, 7, 27, 18, 0, 0), Location = "Pariisi", Channel = "Eurosport", Description = "Legendaarisen pyöräilykilpailun ratkaiseva etappi." },
           new Event { Id = 17, Sport = "Uinti", Name = "Olympialaiset: 200m vapaauinti finaali", Date = new DateTime(2028, 7, 25, 17, 0, 0), Location = "Los Angeles", Channel = "Yle", Description = "Kenen kunto ja nopeus on tällä kertaa ylivertainen?" },
           new Event { Id = 18, Sport = "Mäkihyppy", Name = "Mäkiviikko", Date = new DateTime(2026, 1, 6, 19, 0, 0), Location = "Bischofshofen", Channel = "Eurosport", Description = "Mäkiviikon päätöskilpailu." }
        };

        // Lajit vasemmalle (unchanged)
        var sports = Events.Select(e => e.Sport).Distinct().ToList();
        sports.Insert(0, "Kaikki lajit");
        SportsList.ItemsSource = sports;

        FilteredEvents = new ObservableCollection<Event>();
        BindingContext = this;

        UpdateFilteredEvents("Kaikki lajit");

        // Ladataan tallennetut Notify-arvot (unchanged)
        foreach (var e in Events)
        {
            e.Notify = Preferences.Get($"Notify_{e.Id}", false);
        }

        AppShell.NotificationsChanged += OnNotificationsChanged;
        AppShell.RoleChanged += OnRoleChanged;
    }

    private void OnNotificationsChanged(object sender, EventArgs e)
    {
        RefreshEventVisibilities();
    }
    private void OnRoleChanged(object sender, EventArgs e)
    {
        RefreshEventVisibilities();
        UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
        ForceCollectionRefresh(); 
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshEventVisibilities();  // New: Refresh all visibilities
        UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
        ForceCollectionRefresh();
    }
    private void ForceCollectionRefresh()
    {
        if (FilteredEvents == null) return;
        var temp = new ObservableCollection<Event>(FilteredEvents); 
        FilteredEvents = null;
        OnPropertyChanged(nameof(FilteredEvents));
        FilteredEvents = temp;
        OnPropertyChanged(nameof(FilteredEvents));
    }

    private void RefreshEventVisibilities()
    {
        bool notificationsEnabled = Preferences.Get("NotificationsEnabled", true);
        DateTime now = DateTime.Now;

        foreach (var e in Events)
        {
            e.NotifyVisible = e.Date >= now && notificationsEnabled;
            e.OnPropertyChanged(nameof(e.IsAdminVisible)); 
            e.OnPropertyChanged(nameof(e.NotifyVisible));  
        }
    }

    public static ObservableCollection<Event> GetAllEvents() => Events;

    public void AddAndRefreshEvent(Event newEvent)
    {
        Events.Add(newEvent);
        UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
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

    // Päivitetään FilteredEvents ja luodaan uusi kokoelma, jotta UI päivittyy
    private void UpdateFilteredEvents(string selectedSport)
    {
        var query = Events.Where(ev => showUpcoming ? ev.Date >= DateTime.Now : ev.Date < DateTime.Now);

        if (!string.IsNullOrEmpty(selectedSport) && selectedSport != "Kaikki lajit")
            query = query.Where(ev => ev.Sport == selectedSport);

        FilteredEvents = new ObservableCollection<Event>(query.OrderBy(ev => ev.Date));
    }

    private void OnClockClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Event evt)
        {
            evt.Notify = !evt.Notify;
            Preferences.Set($"Notify_{evt.Id}", evt.Notify);

            if (evt.Notify)
            {
                DisplayAlert("Ilmoitus ajastettu", $"Saat ilmoituksen tapahtumasta {evt.Name}, {evt.Date}", "OK");
                ScheduleNotification(evt);
            }
            else
                CancelNotification(evt);
        }
    }

    private void ScheduleNotification(Event tapahtuma)
    {
        if (!Preferences.Get("NotificationsEnabled", true) || !tapahtuma.Notify) return;

#if WINDOWS
        ShowWindowsNotification(
            "Ottelu tulossa!",
            $"{tapahtuma.Name} alkaa {tapahtuma.Date:dd.MM.yyyy HH:mm}"
        );
#else
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

    // Hover-efektit (sama kuin aiemmin)
    private void OnSportPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Grid grid && grid.Parent is Frame frame)
        {
            frame.Scale = 1.05;
            frame.BackgroundColor = Color.FromArgb("#0066CC");
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

    private void OnButtonPointerEntered(object sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            btn.Scale = 1.05;
            btn.Opacity = 0.9;
        }
    }

    private void OnButtonPointerExited(object sender, PointerEventArgs e)
    {
        if (sender is Button btn)
        {
            btn.Scale = 1.0;
            btn.Opacity = 1.0;
        }
    }

    private async void OnDeleteEventClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Event evt)
        {
            bool confirm = await DisplayAlert("Vahvista poisto", $"Haluatko varmasti poistaa tapahtuman '{evt.Name}'?", "Kyllä", "Peruuta");
            if (confirm)
            {
                Events.Remove(evt);
                UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
            }
        }
    }

    private async void OnEditEventClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Event evt)
        {
            var popup = new EditEventPopup(evt);
            var result = await this.ShowPopupAsync<Event>(popup);

            if (result != null)
            {
                // Päivitetään UI
                UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
