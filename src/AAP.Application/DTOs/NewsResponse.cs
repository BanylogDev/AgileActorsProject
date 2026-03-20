using System.Text.Json.Serialization;
using AAP.Domain.Entities;

namespace AAP.Application.DTOs
{
    public class NewsResponse
    {
        [JsonPropertyName("articles")]
        public List<NewsData> Articles { get; set; } = new();
    }
}