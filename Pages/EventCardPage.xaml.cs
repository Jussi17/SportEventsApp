using Microsoft.Maui.Controls;
using SportEventsApp.Models;

namespace SportEventsApp.Pages
{
    [QueryProperty("SelectedEvent", "SelectedEvent")]
    public partial class EventCardPage : ContentPage
    {
        public Event SelectedEvent { get; set; }
        public EventCardPage()
        {
            InitializeComponent();
        }
    }
}
