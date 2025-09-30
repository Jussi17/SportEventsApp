using Microsoft.Maui.Controls;
using SportEventsApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SportEventsApp.Pages
{
    public partial class CalendarPage : ContentPage
    {
        public ObservableCollection<Event> Events { get; set; }
        public ObservableCollection<Event> FilteredEvents { get; set; }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                FilterEvents();
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public CalendarPage()
        {
            InitializeComponent();

            Events = new ObservableCollection<Event>
            {
                new Event { Name = "Jalkapallo MM-finaali", Date = new DateTime(2026, 7, 12, 21, 0, 0), Location = "New York", Channel = "Yle Areena" },
                new Event { Name = "Olympialaiset - Avajaiset", Date = new DateTime(2028, 7, 20, 20, 0, 0), Location = "Los Angeles", Channel = "Discovery+" },
                new Event { Name = "NHL Winter Classic", Date = new DateTime(2026, 1, 1, 19, 0, 0), Location = "Chicago", Channel = "Viaplay" }
            };

            FilteredEvents = new ObservableCollection<Event>();
            SelectedDate = DateTime.Today;

            BindingContext = this;
        }

        private void FilterEvents()
        {
            FilteredEvents.Clear();
            var eventsOnDate = Events.Where(e => e.Date.Date == SelectedDate.Date);

            foreach (var ev in eventsOnDate)
                FilteredEvents.Add(ev);
        }
    }
}
