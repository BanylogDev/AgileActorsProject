using AAP.Domain.Entities;

namespace AAP.Application.Services
{
    public interface IStatisticsService
    {
        void Record(string apiName, long responseTimeMs);
        List<Statistics> GetStatistics();
    }
}