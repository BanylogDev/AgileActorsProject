using AAP.Application.Services;
using AAP.Application.UseCases;
using AAP.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class GetStatisticsUseCaseTests
{
    [Fact]
    public void Execute_ReturnsStatistics()
    {
        // Arrange
        var statsService = new Mock<IStatisticsService>();
        var logger = new Mock<ILogger<GetStatisticsUseCase>>();

        statsService.Setup(x => x.GetStatistics())
                    .Returns(new List<Statistics>
                    {
                        new Statistics { ApiName = "News", TotalRequests = 5 }
                    });

        var useCase = new GetStatisticsUseCase(statsService.Object, logger.Object);

        // Act
        var result = useCase.Execute();

        // Assert
        Assert.Single(result);
        Assert.Equal("News", result[0].ApiName);
    }
}
