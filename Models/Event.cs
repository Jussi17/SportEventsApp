using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SportEventsApp.Models;

public class Event : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Channel { get; set; }
    public string Description { get; set; }

    private bool _notify;
    public bool Notify
    {
        get => _notify;
        set
        {
            if (_notify == value) return;
            _notify = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(BellImage));
        }
    }

    public string BellImage => Notify ? "bell_filled.png" : "bell.png";

    private bool _notifyVisible;
    public bool NotifyVisible
    {
        get => _notifyVisible;
        set
        {
            if (_notifyVisible == value) return;
            _notifyVisible = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
