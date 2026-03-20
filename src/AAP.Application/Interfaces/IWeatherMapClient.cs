using AAP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AAP.Application.Interfaces
{
    public interface IWeatherMapClient
    {
        Task<WeatherMapData?> GetWeatherAsync(string url);
    }
}
