using Microsoft.Maui.Controls;
using Plugin.Maui.Calendar.Controls;
using Plugin.Maui.Calendar.Models;
using SportEventsApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Plugin.Maui.Calendar.Models;

namespace SportEventsApp.Pages
{
    public partial class CalendarPage : ContentPage
    {
        public ObservableCollection<Event> FilteredEvents { get; set; }
        public EventCollection EventDates { get; set; }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                FilterEvents(); // P‰ivitt‰‰ tapahtumat aina kun valitaan p‰iv‰
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public CalendarPage()
        {
            InitializeComponent();

            FilteredEvents = new ObservableCollection<Event>();
            EventDates = new EventCollection();
            SelectedDate = DateTime.Today;

            ConfigureCalendar();

            BindingContext = this;
        }

        private void ConfigureCalendar()
        {
            if (MyCalendar != null)
            {
                var finnishCulture = new CultureInfo("fi-FI");
                finnishCulture.DateTimeFormat.AbbreviatedDayNames = ["ma", "ti", "ke", "to", "pe", "la", "su"];

                MyCalendar.Culture = finnishCulture;

                // Ladataan tapahtumien p‰iv‰m‰‰r‰t
                UpdateEventDates();
            }
        }

        private void UpdateEventDates()
        {
            EventDates.Clear();
            var allEvents = EventsListPage.Events;
            var eventsByDate = allEvents.GroupBy(e => e.Date.Date);

            foreach (var group in eventsByDate)
            {
                EventDates[group.Key] = new List<object>();
            }

            OnPropertyChanged(nameof(EventDates));  // Ilmoittaa UI:lle muutoksesta
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            UpdateEventDates();
            FilterEvents();

            // Responsiivisuus Androidille
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Pystysuora layout: kalenteri ylˆs, tapahtumat alle
                Grid.SetRow(CalendarStack, 0);
                Grid.SetColumn(CalendarStack, 0);

                Grid.SetRow(EventsStack, 1);
                Grid.SetColumn(EventsStack, 0);

                CalendarColumn.Width = new GridLength(1, GridUnitType.Star);
                EventsColumn.Width = new GridLength(0); // oikea sarake pois
                TopRow.Height = GridLength.Auto;
                BottomRow.Height = GridLength.Star;
            }
            else
            {
                // PC: vaakasuora layout
                Grid.SetRow(CalendarStack, 0);
                Grid.SetColumn(CalendarStack, 0);

                Grid.SetRow(EventsStack, 0);
                Grid.SetColumn(EventsStack, 1);

                CalendarColumn.Width = new GridLength(350);
                EventsColumn.Width = GridLength.Star;
                TopRow.Height = GridLength.Auto;
                BottomRow.Height = GridLength.Star;
            }
        }

        private void FilterEvents()
        {
            FilteredEvents.Clear();
            var allEvents = EventsListPage.Events;
            var eventsOnDate = allEvents.Where(e => e.Date.Date == SelectedDate.Date);

            foreach (var ev in eventsOnDate)
                FilteredEvents.Add(ev);
        }

        private async void OnEventTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame && frame.BindingContext is Event selectedEvent)
            {
                await Navigation.PushAsync(new EventCardPage(selectedEvent));
            }
        }

#if WINDOWS
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
#endif
    }
}
