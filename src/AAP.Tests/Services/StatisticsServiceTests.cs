using AAP.Domain.Entities;
using AAP.Infrastructure.Services;
using Xunit;

public class StatisticsServiceTests
{
    [Fact]
    public void AddRecord_Statistics()
    {
        // Arrange
        var service = new StatisticsService();

        // Act
        service.Record("News", 120);

        // Assert
        var stats = service.GetStatistics();
        Assert.Single(stats);
        Assert.Equal("News", stats[0].ApiName);
        Assert.Equal(1, stats[0].TotalRequests);
        Assert.Equal(120, stats[0].AverageResponseTime);
    }

    [Fact]
    public void GetStatistics_CheckTimes()
    {
        // Arrange
        var service = new StatisticsService();

        service.Record("Weather", 50);  
        service.Record("Weather", 150); 
        service.Record("Weather", 300); 

        // Act
        var stats = service.GetStatistics();
        var weatherStats = stats[0];

        // Assert
        Assert.Equal(3, weatherStats.TotalRequests);
        Assert.Equal(1, weatherStats.FastRequests);
        Assert.Equal(1, weatherStats.MediumRequests);
        Assert.Equal(1, weatherStats.SlowRequests);
    }

    [Fact]
    public void GetStatistics()
    {
        // Arrange
        var service = new StatisticsService();

        // Act
        var stats = service.GetStatistics();

        // Assert
        Assert.Empty(stats);
    }

    [Fact]
    public void AddRecords()
    {
        // Arrange
        var service = new StatisticsService();

        service.Record("News", 100);
        service.Record("Reddit", 200);
        service.Record("News", 300);

        // Act
        var stats = service.GetStatistics();

        // Assert
        Assert.Equal(2, stats.Count);

        var news = stats.First(s => s.ApiName == "News");
        var reddit = stats.First(s => s.ApiName == "Reddit");

        Assert.Equal(2, news.TotalRequests);
        Assert.Equal(1, reddit.TotalRequests);
    }

    [Fact]
    public void GetStatistics_Avrage()
    {
        // Arrange
        var service = new StatisticsService();

        service.Record("Api1", 100);
        service.Record("Api2", 200);
        service.Record("Api3", 300);

        // Act
        var stats = service.GetStatistics();
        var apiStats = stats[0];

        // Assert
        Assert.Equal(200, apiStats.AverageResponseTime);
    }
}
