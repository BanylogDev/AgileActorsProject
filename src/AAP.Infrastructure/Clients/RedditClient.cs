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
    public class RedditClient : IRedditClient
    {
        private readonly HttpClient _httpClient;
        private readonly IStatisticsService _stats;
        private readonly Caching _cache;
        private readonly ILogger<RedditClient> _logger;

        public RedditClient(HttpClient httpClient, IStatisticsService stats, Caching cache, ILogger<RedditClient> logger)
        {
            _httpClient = httpClient;
            _stats = stats;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<RedditData>> GetRedditPostsAsync(string url, string? sortBy = null, string? sortDirection = null)
        {
            _logger.LogInformation("Getting Reddit posts from {Url}", url);

            var stopwatch = Stopwatch.StartNew();
            var cacheKey = $"reddit:{url}";

            var cached = _cache.Get<List<RedditData>>(cacheKey);
            if (cached != null)
            {
                stopwatch.Stop();
                _stats.Record("Reddit", stopwatch.ElapsedMilliseconds);
                _logger.LogInformation("Reddit posts from cache");
                return SortReddit(cached, sortBy, sortDirection);
            }

            if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AAPApiAggregator/1.0");

            var response = await _httpClient.GetAsync(url);

            stopwatch.Stop();
            _stats.Record("Reddit", stopwatch.ElapsedMilliseconds);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error, Reddit api returned unsuccessffull status");
                return new List<RedditData>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var redditResponse = JsonSerializer.Deserialize<RedditResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var posts = redditResponse?.Data?.Children?
                .Select(x => new RedditData
                {
                    Title = x.Data.Title,
                    Author = x.Data.Author,
                    Url = x.Data.Url,
                    CreatedAt = DateTimeOffset.FromUnixTimeSeconds((long)x.Data.Created_Utc).UtcDateTime
                })
                .ToList() ?? new List<RedditData>();

            _cache.Set(cacheKey, posts, 5);

            return SortReddit(posts, sortBy, sortDirection);
        }

        private List<RedditData> SortReddit(List<RedditData> posts, string? sortBy, string? direction)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return posts;

            bool desc = direction == "desc";

            if (sortBy == "date")
                return desc ? posts.OrderByDescending(r => r.CreatedAt).ToList()
                            : posts.OrderBy(r => r.CreatedAt).ToList();

            if (sortBy == "title")
                return desc ? posts.OrderByDescending(r => r.Title).ToList()
                            : posts.OrderBy(r => r.Title).ToList();

            return posts;
        }
    }
}
