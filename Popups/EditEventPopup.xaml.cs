using CommunityToolkit.Maui.Views;
using SportEventsApp.Models;

namespace SportEventsApp.Popups;

public partial class EditEventPopup : Popup<Event>
{
    private Event _event;

    public EditEventPopup(Event evt)
    {
        InitializeComponent();
        _event = evt;

        // T‰ytet‰‰n kent‰t
        NameEntry.Text = evt.Name;
        LocationEntry.Text = evt.Location;
        ChannelEntry.Text = evt.Channel;
        DescriptionEditor.Text = evt.Description;
        DatePicker.Date = evt.Date.Date;
        TimePicker.Time = evt.Date.TimeOfDay;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Suljetaan popup ilman tulosta
        await CloseAsync(null);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // P‰ivitet‰‰n event-olio suoraan
        _event.Name = NameEntry.Text;
        _event.Location = LocationEntry.Text;
        _event.Channel = ChannelEntry.Text;
        _event.Description = DescriptionEditor.Text;
        _event.Date = DatePicker.Date.Add(TimePicker.Time);

        // Suljetaan popup ja palautetaan muokattu event
        await CloseAsync(_event);
    }
}
