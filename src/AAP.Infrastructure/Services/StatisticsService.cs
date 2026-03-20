using AAP.Application.Services;
using AAP.Domain.Entities;
using System.Collections.Concurrent;

namespace AAP.Infrastructure.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ConcurrentDictionary<string, List<long>> _data = new();

        public void Record(string apiName, long responseTimeMs)
        {
            var list = _data.GetOrAdd(apiName, _ => new List<long>());

            lock (list)
            {
                list.Add(responseTimeMs);
            }
        }

        public List<Statistics> GetStatistics()
        {
            var result = new List<Statistics>();

            foreach (var kvp in _data)
            {
                var apiName = kvp.Key;
                var times = kvp.Value;

                int totalRequests;
                double averageResponseTime;
                int fastRequests;
                int mediumRequests;
                int slowRequests;

                lock (times)
                {
                    if (times.Count == 0)
                        continue;

                    totalRequests = times.Count;
                    averageResponseTime = times.Average();
                    fastRequests = times.Count(t => t < 100);
                    mediumRequests = times.Count(t => t >= 100 && t <= 200);
                    slowRequests = times.Count(t => t > 200);
                }

                result.Add(new Statistics
                {
                    ApiName = apiName,
                    TotalRequests = totalRequests,
                    AverageResponseTime = averageResponseTime,
                    FastRequests = fastRequests,
                    MediumRequests = mediumRequests,
                    SlowRequests = slowRequests
                });
            }

            return result;
        }
    }
}