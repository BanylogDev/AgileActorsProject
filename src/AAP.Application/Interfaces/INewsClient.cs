using AAP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AAP.Application.Interfaces
{
    public interface INewsClient
    {
        Task<List<NewsData>> GetNewsAsync(string url, string? sortBy = null, string? sortDirection = null);
    }
}
