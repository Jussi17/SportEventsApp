using Microsoft.Maui.Controls;
using SportEventsApp.Models;

namespace SportEventsApp.Pages
{
    public partial class EventCardPage : ContentPage
    {
        public EventCardPage(Event evt)
        {
            InitializeComponent();
            BindingContext = evt;
        }
    }
}