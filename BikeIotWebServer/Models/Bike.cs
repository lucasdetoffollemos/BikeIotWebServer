namespace BikeIotWebServer.Models
{
    public class Bike
    {
        public double Speed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BatteryLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
