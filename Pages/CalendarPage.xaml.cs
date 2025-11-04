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
                // Asetetaan suomen kieli (ma = Monday ensimm‰inen p‰iv‰)
                var finnishCulture = new CultureInfo("fi-FI");
                finnishCulture.DateTimeFormat.AbbreviatedDayNames = new[] { "ma", "ti", "ke", "to", "pe", "la", "su" };

                // Jos plugin tukee Culture-propertiota:
                MyCalendar.Culture = finnishCulture;

                // Ladataan tapahtumien p‰iv‰m‰‰r‰t
                UpdateEventDates();
            }
        }

        private void UpdateEventDates()
        {
            EventDates.Clear();
            var allEvents = EventsListPage.Events;

            // Ryhmittele tapahtumat p‰iv‰m‰‰r‰n mukaan
            var eventsByDate = allEvents.GroupBy(e => e.Date.Date);

            foreach (var group in eventsByDate)
            {
                // Dictionary-syntaksi: avain = p‰iv‰m‰‰r‰, arvo = tyhj‰ lista
                EventDates[group.Key] = new List<object>();
            }

            OnPropertyChanged(nameof(EventDates));  // Ilmoittaa UI:lle muutoksesta
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateEventDates();
            FilterEvents();
        }

        private void FilterEvents()
        {
            FilteredEvents.Clear();
            var allEvents = EventsListPage.Events;
            var eventsOnDate = allEvents.Where(e => e.Date.Date == SelectedDate.Date);

            foreach (var ev in eventsOnDate)
                FilteredEvents.Add(ev);
        }
        private async void OnEventSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Event selectedEvent)
            {
                ((CollectionView)sender).SelectedItem = null;

                await Navigation.PushAsync(new EventCardPage(selectedEvent));
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
    }
}
