namespace BikeIotWebServer.Models
{
    public class Bike
    {
        public Guid Id { get; set; }
        public double Speed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
