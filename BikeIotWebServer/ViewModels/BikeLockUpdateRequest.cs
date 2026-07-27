namespace BikeIotWebServer.ViewModels
{
    public class BikeLockUpdateRequest
    {
        public int BikeId { get; set; }
        public bool IsLock { get; set; }
    }
}
