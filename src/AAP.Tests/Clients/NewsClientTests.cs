using AAP.Application.Services;
using AAP.Domain.Entities;
using AAP.Infrastructure.Clients;
using AAP.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using Xunit;

public class NewsClientTests
{

    [Fact]
    public async Task GetNewsAsync_ReturnsArticless()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            articles = new[]
            {
                new { title = "Test", description = "desc", url = "test.com", publishedAt = DateTime.UtcNow }
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
        var logger = new Mock<ILogger<NewsClient>>();

        var client = new NewsClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetNewsAsync("http://test.com");

        // Assert
        Assert.Single(result);
        Assert.Equal("Test", result[0].Title);
    }

    [Fact]
    public async Task GetNewsAsync_ReturnsEmptyList()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);

        var statsMock = new Mock<IStatisticsService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new Caching(memoryCache);
        var logger = new Mock<ILogger<NewsClient>>();

        var client = new NewsClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetNewsAsync("http://test.com");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetNewsAsync_UsesCache()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            articles = new[]
            {
                new { title = "Test", description = "desc", url = "test.com", publishedAt = DateTime.UtcNow }
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
        var logger = new Mock<ILogger<NewsClient>>();

        var client = new NewsClient(httpClient, statsMock.Object, cache, logger.Object);

        await client.GetNewsAsync("http://test.com");

        var result = await client.GetNewsAsync("http://test.com");

        // Assert
        Assert.Single(result);
        Assert.Equal("Test", result[0].Title);
    }

    [Fact]
    public async Task GetNews_SortsByDateDescending()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            articles = new[]
            {
                new { title = "OldNewsTest", description = "descTest1", url = "test1.com", publishedAt = DateTime.UtcNow.AddDays(-1) },
                new { title = "NewNewsTest", description = "descTest2", url = "test2.com", publishedAt = DateTime.UtcNow }
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
        var logger = new Mock<ILogger<NewsClient>>();

        var client = new NewsClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetNewsAsync("http://test.com", "date", "desc");

        // Assert
        Assert.Equal("NewNewsTest", result[0].Title);
    }
}
