using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using SportEventsApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

#if WINDOWS
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
#endif

namespace SportEventsApp.Pages;

public partial class EventsListPage : ContentPage
{
    public ObservableCollection<Event> Events { get; set; }

    public EventsListPage()
    {
        InitializeComponent();

        Events = new ObservableCollection<Event>
        {
            new Event { Id = 1, Name = "Testitapahtuma", Date = DateTime.Now.AddMinutes(1), Location = "Helsinki", Channel = "MTV3" },
            new Event { Id = 2, Name = "Jalkapallo MM-finaali", Date = new DateTime(2026,7,12,21,0,0), Location = "New York", Channel = "Yle Areena" },
            new Event { Id = 3, Name = "Olympialaiset - Avajaiset", Date = new DateTime(2028,7,20,20,0,0), Location = "Los Angeles", Channel = "Discovery+" }
        };

        // Ladataan tallennetut Notify-arvot
        foreach (var e in Events)
        {
            e.Notify = Preferences.Get($"Notify_{e.Id}", false);
            e.NotifyVisible = Preferences.Get("NotificationsEnabled", true);
        }

        BindingContext = this;
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
                DisplayAlert("Ilmoitus ajastettu", $"Saat ilmoituksen tapahtumasta {evt.Name} klo {notifyTime:HH:mm}", "OK");
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
    }
}
