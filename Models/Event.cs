using SportEventsApp.Helpers;
using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SportEventsApp.Models;

public class Event : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Sport { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string Channel { get; set; }
    public string Description { get; set; }
    public bool IsAdminVisiblePc { get; set; }
    public bool IsAdminVisible
    {
        get => UserRoleHelper.IsAdmin;
        set { OnPropertyChanged(); }  
    }

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
    public string BackArrow = "nuoli.png";

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
    public void OnPropertyChanged([CallerMemberName] string propName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}