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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
#endif

namespace SportEventsApp.Pages;

public partial class EventsListPage : ContentPage, INotifyPropertyChanged
{
    private EventRepository repo = new EventRepository();
    public static ObservableCollection<Event> Events { get; set; } = new ObservableCollection<Event>();
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
        LoadEventsDb();
        CurrentInstance = this;

        System.Diagnostics.Debug.WriteLine($"IsLoggedIn: {Preferences.Get("IsLoggedIn", false)}");
        System.Diagnostics.Debug.WriteLine($"Role: {Preferences.Get("Role", "NOT_SET")}");
        System.Diagnostics.Debug.WriteLine($"Username: {Preferences.Get("Username", "NOT_SET")}");

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

    private void OnPageSizeChanged(object sender, EventArgs e)
    {
        if (EventsPageRoot.Width < 600) // mobiili
        {
            Grid.SetColumn(SportsList, 0);
            Grid.SetColumnSpan(SportsList, 2);

            Grid.SetRow(EventsList, 2);
            Grid.SetColumn(EventsList, 0);
            Grid.SetColumnSpan(EventsList, 2);

            if (EventsList.ItemsLayout is GridItemsLayout layout)
                layout.Span = 1;
        }
        else // tabletti / vaakasuora
        {
            Grid.SetColumn(SportsList, 0);
            Grid.SetColumnSpan(SportsList, 1);

            Grid.SetRow(EventsList, 1);
            Grid.SetColumn(EventsList, 1);
            Grid.SetColumnSpan(EventsList, 1);

            if (EventsList.ItemsLayout is GridItemsLayout layout)
                layout.Span = 2;
        }
    }

    public void LoadEventsDb()
    {
        Events.Clear();
        foreach (var e in repo.GetAllEvents())
        {
            Events.Add(e);
        }
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
        repo.InsertEvent(newEvent);
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
        if (!Preferences.Get("NotificationsEnabled", true) || !tapahtuma.Notify)
            return;

#if WINDOWS
    ShowWindowsNotification(
        "Ottelu tulossa!",
        $"{tapahtuma.Name} alkaa {tapahtuma.Date:dd.MM.yyyy HH:mm}"
    );
#else
        // Muistutus mobiililaitteille
        var notification = new NotificationRequest
        {
            NotificationId = tapahtuma.Id,
            Title = "Ottelu tulossa!",
            Description = $"{tapahtuma.Name} alkaa {tapahtuma.Date:dd.MM.yyyy HH:mm}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = tapahtuma.Date // Ilmoitus tapahtuma-aikaan
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
        if (DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            if (sender is Button btn)
            {
                btn.Scale = 1.05;
                btn.Opacity = 0.9;
            }
        }
    }

    private void OnButtonPointerExited(object sender, PointerEventArgs e)
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            if (sender is Button btn)
            {
                btn.Scale = 1.0;
                btn.Opacity = 1.0;
            }
        }
    }

    private async void OnDeleteEventClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Event evt)
        {
            bool confirm = await DisplayAlert("Vahvista poisto", $"Haluatko varmasti poistaa tapahtuman '{evt.Name}'?", "Kyllä", "Peruuta");
            if (confirm)
            {
                repo.DeleteEvent(evt);
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
            var popupResult = await this.ShowPopupAsync<Event>(popup);
            var result = popupResult.Result;

            if (result != null)
            {
                // Päivitetään UI
                repo.UpdateEvent(result);
                UpdateFilteredEvents(SportsList.SelectedItem as string ?? "Kaikki lajit");
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
