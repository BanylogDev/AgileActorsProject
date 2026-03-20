using System.Diagnostics;
using System.Text.Json;
using AAP.Application.DTOs;
using AAP.Application.Interfaces;
using AAP.Application.Services;
using AAP.Domain.Entities;
using AAP.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace AAP.Infrastructure.Clients
{
    public class WeatherMapClient : IWeatherMapClient
    {
        private readonly HttpClient _httpClient;
        private readonly IStatisticsService _stats;
        private readonly Caching _cache;
        private readonly ILogger<WeatherMapClient> _logger;

        public WeatherMapClient(HttpClient httpClient, IStatisticsService stats, Caching cache, ILogger<WeatherMapClient> logger)
        {
            _httpClient = httpClient;
            _stats = stats;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeatherMapData?> GetWeatherAsync(string url)
        {
            _logger.LogInformation("Getting weather data from {Url}", url);

            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"weather:{url}";

            var cached = _cache.Get<WeatherMapData>(cacheKey);
            if (cached != null)
            {
                stopwatch.Stop();
                _stats.Record("Weather", stopwatch.ElapsedMilliseconds);
                _logger.LogInformation("Weather from cache");
                return cached;
            }

            var response = await _httpClient.GetAsync(url);

            stopwatch.Stop();
            _stats.Record("Weather", stopwatch.ElapsedMilliseconds);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error, Weather api returned unsuccessfull status");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<WeatherResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var weather = result?.CurrentWeather;

            if (weather != null)
                _cache.Set(cacheKey, weather, 5);

            return weather;
        }
    }
}
