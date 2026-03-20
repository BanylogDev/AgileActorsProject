using AAP.Application.DTOs;
using AAP.Application.Interfaces;
using AAP.Application.UseCases;
using AAP.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class GetAggregatedDataUseCaseTests
{
    [Fact]
    public async Task Execute_ReturnsData()
    {
        //Arrange
        var newsClient = new Mock<INewsClient>();
        var redditClient = new Mock<IRedditClient>();
        var weatherClient = new Mock<IWeatherMapClient>();
        var logger = new Mock<ILogger<GetAggregatedDataUseCase>>();

        var newsList = new List<NewsData>
        {
            new NewsData { Title = "NewsTest", PublishedAt = DateTime.UtcNow }
        };

        var redditList = new List<RedditData>
        {
            new RedditData { Title = "RedditTest", CreatedAt = DateTime.UtcNow }
        };

        var weather = new WeatherMapData { Temperature = 10 };

        newsClient.Setup(x => x.GetNewsAsync(It.IsAny<string>(), null, null))
                  .ReturnsAsync(newsList);

        redditClient.Setup(x => x.GetRedditPostsAsync(It.IsAny<string>(), null, null))
                    .ReturnsAsync(redditList);

        weatherClient.Setup(x => x.GetWeatherAsync(It.IsAny<string>()))
                     .ReturnsAsync(weather);

        var useCase = new GetAggregatedDataUseCase(
            newsClient.Object,
            redditClient.Object,
            weatherClient.Object,
            logger.Object
        );

        var query = new AggregatedQuery();

        //Act

        var result = await useCase.Execute("news", "reddit", "weather", query);


        //Assert

        Assert.Single(result.News);
        Assert.Single(result.RedditPosts);
        Assert.Equal(10, result.Weather.Temperature);
    }

    [Fact]
    public async Task Execute_FiltersNewsByDate()
    {
        var newsClient = new Mock<INewsClient>();
        var redditClient = new Mock<IRedditClient>();
        var weatherClient = new Mock<IWeatherMapClient>();
        var logger = new Mock<ILogger<GetAggregatedDataUseCase>>();

        var newsList = new List<NewsData>
        {
            new NewsData { Title = "Old", PublishedAt = DateTime.UtcNow.AddDays(-5) },
            new NewsData { Title = "New", PublishedAt = DateTime.UtcNow }
        };

        newsClient.Setup(x => x.GetNewsAsync(It.IsAny<string>(), null, null))
                  .ReturnsAsync(newsList);

        redditClient.Setup(x => x.GetRedditPostsAsync(It.IsAny<string>(), null, null))
                    .ReturnsAsync(new List<RedditData>());

        weatherClient.Setup(x => x.GetWeatherAsync(It.IsAny<string>()))
                     .ReturnsAsync(new WeatherMapData());

        var useCase = new GetAggregatedDataUseCase(
            newsClient.Object,
            redditClient.Object,
            weatherClient.Object,
            logger.Object
        );

        var query = new AggregatedQuery
        {
            FromDate = DateTime.UtcNow.AddDays(-1)
        };

        var result = await useCase.Execute("news", "reddit", "weather", query);

        Assert.Single(result.News);
        Assert.Equal("New", result.News[0].Title);
    }

}
