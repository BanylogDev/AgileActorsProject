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
    public class NewsClient : INewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly IStatisticsService _stats;
        private readonly Caching _cache;
        private readonly ILogger<NewsClient> _logger;

        public NewsClient(HttpClient httpClient, IStatisticsService stats, Caching cache, ILogger<NewsClient> logger)
        {
            _httpClient = httpClient;
            _stats = stats;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<NewsData>> GetNewsAsync(string url, string? sortBy = null, string? sortDirection = null)
        {
            _logger.LogInformation("Getting news from {Url}", url);

            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"news:{url}";

            var cached = _cache.Get<List<NewsData>>(cacheKey);
            if (cached != null)
            {
                stopwatch.Stop();
                _stats.Record("News", stopwatch.ElapsedMilliseconds);
                _logger.LogInformation("News from cache");
                return SortNews(cached, sortBy, sortDirection);
            }

            if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AAPApiAggregator/1.0");

            var response = await _httpClient.GetAsync(url);

            stopwatch.Stop();
            _stats.Record("News", stopwatch.ElapsedMilliseconds);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error, News api returned unsuccessffull status");
                return new List<NewsData>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<NewsResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var articles = result?.Articles ?? new List<NewsData>();

            _cache.Set(cacheKey, articles, 5);

            return SortNews(articles, sortBy, sortDirection);
        }

        private List<NewsData> SortNews(List<NewsData> news, string? sortBy, string? direction)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return news;

            bool desc = direction == "desc";

            if (sortBy == "date")
                return desc ? news.OrderByDescending(n => n.PublishedAt).ToList()
                            : news.OrderBy(n => n.PublishedAt).ToList();

            if (sortBy == "title")
                return desc ? news.OrderByDescending(n => n.Title).ToList()
                            : news.OrderBy(n => n.Title).ToList();

            return news;
        }
    }
}
