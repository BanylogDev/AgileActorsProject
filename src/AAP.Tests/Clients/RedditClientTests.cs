using AAP.Application.Services;
using AAP.Domain.Entities;
using AAP.Infrastructure.Clients;
using AAP.Infrastructure.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Xunit;

public class RedditClientTests
{

    [Fact]
    public async Task GetRedditPostsAsync_ReturnsPosts()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            data = new
            {
                children = new[]
                {
                    new
                    {
                        data = new
                        {
                            title = "PostTest",
                            author = "UserAgileActors",
                            url = "http://reddit.com/1",
                            created_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    }
                }
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

        var logger = new Mock<ILogger<RedditClient>>();

        var client = new RedditClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetRedditPostsAsync("http://test.com");

        // Assert
        Assert.Single(result);
        Assert.Equal("PostTest", result[0].Title);
        Assert.Equal("UserAgileActors", result[0].Author);
    }

    [Fact]
    public async Task GetRedditPostsAsync_ReturnsEmptyList()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler);

        var statsMock = new Mock<IStatisticsService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cache = new Caching(memoryCache);
        var logger = new Mock<ILogger<RedditClient>>();

        var client = new RedditClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetRedditPostsAsync("http://test.com");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRedditPostsAsync_UsesCache()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            data = new
            {
                children = new[]
                {
                    new
                    {
                        data = new
                        {
                            title = "CachedPost",
                            author = "UserAgileActors",
                            url = "http://reddit.com/1",
                            created_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    }
                }
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

        var logger = new Mock<ILogger<RedditClient>>();

        var client = new RedditClient(httpClient, statsMock.Object, cache, logger.Object);

        await client.GetRedditPostsAsync("http://test.com");

        var result = await client.GetRedditPostsAsync("http://test.com");

        // Assert
        Assert.Single(result);
        Assert.Equal("CachedPost", result[0].Title);
    }

    [Fact]
    public async Task GetRedditPostsAsync_SortsByDateDescending()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            data = new
            {
                children = new[]
                {
                    new
                    {
                        data = new
                        {
                            title = "OldPost",
                            author = "UserAgileActors",
                            url = "http://reddit.com/old",
                            created_utc = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds()
                        }
                    },
                    new
                    {
                        data = new
                        {
                            title = "NewPost",
                            author = "UserBAgileActors",
                            url = "http://reddit.com/new",
                            created_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    }
                }
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

        var logger = new Mock<ILogger<RedditClient>>();

        var client = new RedditClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetRedditPostsAsync("http://test.com", "date", "desc");

        // Assert
        Assert.Equal("NewPost", result[0].Title);
    }

    [Fact]
    public async Task GetRedditPostsAsync_SortsByTitleAscending()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new
        {
            data = new
            {
                children = new[]
                {
                    new
                    {
                        data = new
                        {
                            title = "Test1",
                            author = "UserAgileActors",
                            url = "http://reddit.com/z",
                            created_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    },
                    new
                    {
                        data = new
                        {
                            title = "Test2",
                            author = "UserBAgileActors",
                            url = "http://reddit.com/a",
                            created_utc = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    }
                }
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

        var logger = new Mock<ILogger<RedditClient>>();

        var client = new RedditClient(httpClient, statsMock.Object, cache, logger.Object);

        // Act
        var result = await client.GetRedditPostsAsync("http://test.com", "title", "asc");

        // Assert
        Assert.Equal("Test1", result[0].Title);
    }
}
