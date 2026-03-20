using System.Text.Json.Serialization;
using AAP.Domain.Entities;

namespace AAP.Application.DTOs
{
    public class WeatherResponse
    {
        [JsonPropertyName("current_weather")]
        public WeatherMapData? CurrentWeather { get; set; }
    }
}