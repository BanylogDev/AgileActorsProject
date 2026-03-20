using AAP.Application.DTOs;
using AAP.Application.Interfaces;
using AAP.Application.UseCases.Interfaces;
using AAP.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace AAP.Application.UseCases
{
    public class GetAggregatedDataUseCase : IGetAggregatedDataUseCase
    {
        private readonly INewsClient _newsClient;
        private readonly IRedditClient _redditClient;
        private readonly IWeatherMapClient _weatherMapClient;
        private readonly ILogger<GetAggregatedDataUseCase> _logger;

        public GetAggregatedDataUseCase(
            INewsClient newsClient,
            IRedditClient redditClient,
            IWeatherMapClient weatherMapClient,
            ILogger<GetAggregatedDataUseCase> logger)
        {
            _newsClient = newsClient;
            _redditClient = redditClient;
            _weatherMapClient = weatherMapClient;
            _logger = logger;
        }

        public async Task<AggregatedItem> Execute(
            string newsUrl,
            string redditUrl,
            string weatherUrl,
            AggregatedQuery query)
        {
            _logger.LogInformation("Starting getting aggregated data");

            var result = new AggregatedItem();

            var newsTask = _newsClient.GetNewsAsync(newsUrl);
            var redditTask = _redditClient.GetRedditPostsAsync(redditUrl);
            var weatherTask = _weatherMapClient.GetWeatherAsync(weatherUrl);

            try
            {
                await Task.WhenAll(newsTask, redditTask, weatherTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "One or more api calls failed");
                result.Errors.Add(ex.Message);
            }


            // News
            if (newsTask.IsCompletedSuccessfully)
            {
                var news = newsTask.Result;

                // Filter
                if (query.FromDate.HasValue)
                    news = news.Where(n => n.PublishedAt >= query.FromDate.Value).ToList();

                if (query.ToDate.HasValue)
                    news = news.Where(n => n.PublishedAt <= query.ToDate.Value).ToList();

                // Sort
                if (query.SortBy == "date")
                {
                    if (query.SortDirection == "desc")
                        news = news.OrderByDescending(n => n.PublishedAt).ToList();
                    else
                        news = news.OrderBy(n => n.PublishedAt).ToList();
                }
                else if (query.SortBy == "title")
                {
                    if (query.SortDirection == "desc")
                        news = news.OrderByDescending(n => n.Title).ToList();
                    else
                        news = news.OrderBy(n => n.Title).ToList();
                }

                result.News = news;
            }
            else
            {
                result.News = new List<NewsData>();
            }


            // Reddit
            if (redditTask.IsCompletedSuccessfully)
            {
                var reddit = redditTask.Result;

                // Filter
                if (!string.IsNullOrWhiteSpace(query.Author))
                    reddit = reddit.Where(r => r.Author == query.Author).ToList();

                if (query.FromDate.HasValue)
                    reddit = reddit.Where(r => r.CreatedAt >= query.FromDate.Value).ToList();

                if (query.ToDate.HasValue)
                    reddit = reddit.Where(r => r.CreatedAt <= query.ToDate.Value).ToList();

                // Sort
                if (query.SortBy == "date")
                {
                    if (query.SortDirection == "desc")
                        reddit = reddit.OrderByDescending(r => r.CreatedAt).ToList();
                    else
                        reddit = reddit.OrderBy(r => r.CreatedAt).ToList();
                }
                else if (query.SortBy == "title")
                {
                    if (query.SortDirection == "desc")
                        reddit = reddit.OrderByDescending(r => r.Title).ToList();
                    else
                        reddit = reddit.OrderBy(r => r.Title).ToList();
                }

                result.RedditPosts = reddit;
            }
            else
            {
                result.RedditPosts = new List<RedditData>();
            }


            // Weather
            if (weatherTask.IsCompletedSuccessfully)
            {
                result.Weather = weatherTask.Result;
            }
            else
            {
                result.Weather = new WeatherMapData
                {
                    Temperature = 0,
                    WindSpeed = 0,
                    WeatherCode = -1,
                    Time = DateTime.UtcNow
                };
            }

            return result;
        }
    }
}
