using Microsoft.Maui.Controls;
using SportEventsApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SportEventsApp.Pages
{
    public partial class PalveluPage : ContentPage
    {
        public ObservableCollection<Palvelu> Palvelut { get; set; }
        public ICommand OpenPalveluCommand { get; }

        public PalveluPage()
        {
            InitializeComponent();

            Palvelut = new ObservableCollection<Palvelu>
            {
                new Palvelu { Name="Viaplay", Logo="viaplay.png", Url="https://viaplay.fi" },
                new Palvelu { Name="Yle Areena", Logo="yleareena.png", Url="https://areena.yle.fi" },
                new Palvelu { Name="Paramount+", Logo="paramount.png", Url="https://www.paramountplus.com" },
                new Palvelu { Name="C More", Logo="cmore.png", Url="https://www.cmore.fi" },
                new Palvelu { Name="Eurosport", Logo="eurosport.png", Url="https://www.eurosport.com" },
                new Palvelu { Name="Discovery+", Logo="discovery.png", Url="https://www.discoveryplus.com" },
                new Palvelu { Name="Ruutu+", Logo="ruutu.png", Url="https://www.ruutu.fi/plus" },
                new Palvelu { Name="Prime Video", Logo="prime.png", Url="https://www.primevideo.com" }
            };

            OpenPalveluCommand = new Command<string>(async (url) =>
            {
                if (!string.IsNullOrEmpty(url))
                    await Launcher.Default.OpenAsync(url);
            });

            BindingContext = this;
        }
        private void OnPointerEntered(object sender, PointerEventArgs e)
        {
            if (sender is Grid grid && grid.Parent is Frame frame)
            {
                frame.Scale = 1.05;
                frame.Opacity = 0.9;
            }
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            if (sender is Grid grid && grid.Parent is Frame frame)
            {
                frame.Scale = 1.0;
                frame.Opacity = 1.0;
            }
        }
    }
}
