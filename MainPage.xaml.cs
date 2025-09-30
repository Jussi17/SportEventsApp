using System.Collections.ObjectModel;

namespace SportEventsApp;

public partial class MainPage : ContentPage
{
    public ObservableCollection<SportEvent> Events { get; set; }

    public MainPage()
    {
        InitializeComponent();

        // Mock-data
        Events = new ObservableCollection<SportEvent>
        {
            new SportEvent { Name="Jääkiekon MM-finaali", Location="Praha, Tšekki", Date=new DateTime(2025,5,25,20,0,0), Channel="MTV3" },
            new SportEvent { Name="Olympialaiset - 100m juoksu", Location="Los Angeles, USA", Date=new DateTime(2028,7,28,18,0,0), Channel="Yle Areena" },
            new SportEvent { Name="Jalkapallon EM 2028 avausottelu", Location="Iso-Britannia", Date=new DateTime(2028,6,12,21,0,0), Channel="Viaplay" }
        };

        BindingContext = this;
    }
}

public class SportEvent
{
    public string Name { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public string Channel { get; set; }
}
