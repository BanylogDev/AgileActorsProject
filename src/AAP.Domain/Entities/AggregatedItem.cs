using System;
using System.Collections.Generic;
using System.Text;

namespace AAP.Domain.Entities
{
    public class AggregatedItem
    {
        public WeatherMapData? Weather { get; set; }

        public List<NewsData> News { get; set; } = new();

        public List<RedditData> RedditPosts { get; set; } = new();

        public List<string> Errors { get; set; } = new();
    }
}
