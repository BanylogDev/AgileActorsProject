using System.Net;
using System.Net.Http;
using System.Text.Json;
using AAP.Domain.Entities;
using AAP.Infrastructure.Clients;
using AAP.Infrastructure.Services;
using AAP.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class WeatherMapClientTests
{
    [Fact]
    public async Task GetWeatherAsync_ReturnsWeather()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            current_Weather = new
            {
                temperature = 22.5,
                windspeed = 5.2,
                weathercode = 3,
                time = DateTime.UtcNow
            }
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };

        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);

        var statsMock = new Mock<IStatisticsService>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new Caching(memoryCache);

        var logger = new Mock<ILogger<WeatherMapClient>>();

        var client = new WeatherMapClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetWeatherAsync("http://test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(22.5, result.Temperature);
        Assert.Equal(5.2, result.WindSpeed);
        Assert.Equal(3, result.WeatherCode);
    }

    [Fact]
    public async Task GetWeatherAsync_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);

        var statsMock = new Mock<IStatisticsService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new Caching(memoryCache);
        var logger = new Mock<ILogger<WeatherMapClient>>();

        var client = new WeatherMapClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetWeatherAsync("http://test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetWeatherAsync_UsesCache()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            current_Weather = new
            {
                temperature = 10.0,
                windspeed = 1.0,
                weathercode = 1,
                time = DateTime.UtcNow
            }
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };

        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);

        var statsMock = new Mock<IStatisticsService>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new Caching(memoryCache);

        var logger = new Mock<ILogger<WeatherMapClient>>();

        var client = new WeatherMapClient(httpClient, statsMock.Object, cache, logger.Object);

        var first = await client.GetWeatherAsync("http://test.com");

        var second = await client.GetWeatherAsync("http://test.com");

        // Assert
        Assert.NotNull(second);
        Assert.Equal(first.Temperature, second.Temperature);
        Assert.Equal(first.WindSpeed, second.WindSpeed);
    }

    [Fact]
    public async Task GetWeatherAsync_Success()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var json = JsonSerializer.Serialize(new
        {
            current_Weather = new
            {
                temperature = 15.3,
                windspeed = 3.3,
                weathercode = 2,
                time = now
            }
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        };

        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);

        var statsMock = new Mock<IStatisticsService>();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new Caching(memoryCache);

        var logger = new Mock<ILogger<WeatherMapClient>>();

        var client = new WeatherMapClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetWeatherAsync("http://test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15.3, result.Temperature);
        Assert.Equal(3.3, result.WindSpeed);
        Assert.Equal(2, result.WeatherCode);
        Assert.Equal(now, result.Time);
    }
}
