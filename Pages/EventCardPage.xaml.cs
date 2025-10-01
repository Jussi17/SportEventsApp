using Microsoft.Maui.Controls;
using SportEventsApp.Models;

namespace SportEventsApp.Pages
{
    [QueryProperty(nameof(SelectedEvent), nameof(SelectedEvent))]
    public partial class EventCardPage : ContentPage
    {
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                BindingContext = value;
            }
        }
        private Event _selectedEvent;

        public EventCardPage()
        {
            InitializeComponent();
        }
    }
}
