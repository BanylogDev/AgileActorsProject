using System.Text.Json.Serialization;

namespace AAP.Domain.Entities
{
    public class WeatherMapData
    {
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("windspeed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("weathercode")]
        public int WeatherCode { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }
    }
}