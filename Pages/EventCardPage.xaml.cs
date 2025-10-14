using Microsoft.Maui.Controls;
using SportEventsApp.Models;
using System.Threading.Tasks;

namespace SportEventsApp.Pages
{
    public partial class EventCardPage : ContentPage
    {
        public EventCardPage(Event evt)
        {
            InitializeComponent();
            BindingContext = evt;
        }

        private async void BackButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}