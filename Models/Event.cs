namespace SportEventsApp.Models
{
    public class Event
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Channel { get; set; }
        public string Description { get; set; } // Valinnainen, esim. lisätiedot
    }
}
