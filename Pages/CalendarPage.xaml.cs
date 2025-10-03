using Microsoft.Maui.Controls;
using SportEventsApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Globalization;
using Plugin.Maui.Calendar.Controls;

namespace SportEventsApp.Pages
{
    public partial class CalendarPage : ContentPage
    {
        public ObservableCollection<Event> FilteredEvents { get; set; }
        public ObservableCollection<DateTime> EventDates { get; set; }

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
            EventDates = new ObservableCollection<DateTime>();
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
            var eventDates = allEvents.Select(e => e.Date.Date).Distinct();
            foreach (var date in eventDates)
            {
                EventDates.Add(date);
            }
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
    }
}
