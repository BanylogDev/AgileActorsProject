using AAP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AAP.Application.Interfaces
{
    public interface IRedditClient
    {
        Task<List<RedditData>> GetRedditPostsAsync(string url, string? sortBy = null, string? sortDirection = null);
    }
}
