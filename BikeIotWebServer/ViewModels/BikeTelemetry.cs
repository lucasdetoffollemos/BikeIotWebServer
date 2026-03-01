using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BikeIotWebServer.ViewModels
{
    public class BikeTelemetry
    {
        
        [Required]
        [JsonPropertyName("velocidade")]
        public float Velocidade { get; set; }

        [Required]
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }

        [Required]
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }

        [Required]
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    public class Position
    {
        [Required]
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }

        [Required]
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
    }

    public class TelemetryTimestamp
    {
        [Required]
        [JsonPropertyName("dia")]
        public int Dia { get; set; }

        [Required]
        [JsonPropertyName("mes")]
        public int Mes { get; set; }

        [Required]
        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [Required]
        [JsonPropertyName("hora")]
        public int Hora { get; set; }

        [Required]
        [JsonPropertyName("minuto")]
        public int Minuto { get; set; }

        [Required]
        [JsonPropertyName("segundo")]
        public int Segundo { get; set; }
    }

}
