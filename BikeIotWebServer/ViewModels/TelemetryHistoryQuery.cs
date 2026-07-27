using System.ComponentModel.DataAnnotations;

namespace BikeIotWebServer.ViewModels
{
    public class TelemetryHistoryQuery
    {
        public int? BikeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        [Range(1, 500)]
        public int Limit { get; set; } = 100;

        [Range(0, int.MaxValue)]
        public int Offset { get; set; }

        [RegularExpression("^(asc|desc)$", ErrorMessage = "Order must be 'asc' or 'desc'.")]
        public string Order { get; set; } = "desc";
    }
}
