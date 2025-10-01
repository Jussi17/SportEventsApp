using Microsoft.Maui.Controls;
using SportEventsApp.Models;
using System.Collections.ObjectModel;

namespace SportEventsApp.Pages
{
    public partial class EventsListPage : ContentPage
    {
        public ObservableCollection<Event> Events { get; set; }

        public EventsListPage()
        {
            InitializeComponent();

            Events = new ObservableCollection<Event>
            {
                new Event { Name = "Jalkapallo MM-finaali", Date = new DateTime(2026, 7, 12, 21, 0, 0), Location = "New York", Channel = "Yle Areena" },
                new Event { Name = "Olympialaiset - Avajaiset", Date = new DateTime(2028, 7, 20, 20, 0, 0), Location = "Los Angeles", Channel = "Discovery+" },
                new Event { Name = "NHL Winter Classic", Date = new DateTime(2026, 1, 1, 19, 0, 0), Location = "Chicago", Channel = "Viaplay" }
            };

            BindingContext = this;
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Event selectedEvent)
            {
                await Shell.Current.GoToAsync("EventCardPage", new Dictionary<string, object>
        {
            { "SelectedEvent", selectedEvent }
        });

                ((CollectionView)sender).SelectedItem = null;
            }
        }

    }
}
