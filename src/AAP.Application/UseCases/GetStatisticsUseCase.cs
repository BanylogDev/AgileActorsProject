using AAP.Application.Services;
using AAP.Application.UseCases.Interfaces;
using AAP.Domain.Entities;
using Microsoft.Extensions.Logging;

public class GetStatisticsUseCase : IGetStatisticsUseCase
{
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<GetStatisticsUseCase> _logger;

    public GetStatisticsUseCase(
        IStatisticsService statisticsService,
        ILogger<GetStatisticsUseCase> logger)
    {
        _statisticsService = statisticsService;
        _logger = logger;
    }

    public List<Statistics> Execute()
    {
        _logger.LogInformation("Getting statistics");

        var stats = _statisticsService.GetStatistics();

        _logger.LogInformation("Statistics retrieved successffully, Count: {Count}", stats.Count);

        return stats;
    }
}
