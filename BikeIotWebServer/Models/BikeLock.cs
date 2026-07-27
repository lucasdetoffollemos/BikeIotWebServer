namespace BikeIotWebServer.Models
{
    public class BikeLock
    {
        public Guid Id { get; set; }
        public int BikeId { get; set; }
        public bool IsLock { get; set; }
    }
}
