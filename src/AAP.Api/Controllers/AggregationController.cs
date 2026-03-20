using AAP.Application.DTOs;
using AAP.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AAP.Api.Controllers
{
    [ApiController]
    [Route("api/aggregated")]
    //[Authorize]
    public class AggregationController : ControllerBase
    {
        private readonly ILogger<AggregationController> _logger;
        private readonly IConfiguration _config;
        private readonly IGetAggregatedDataUseCase _getAggregatedDataUseCase;

        public AggregationController(
            ILogger<AggregationController> logger,
            IConfiguration config,
            IGetAggregatedDataUseCase getAggregatedDataUseCase)
        {
            _logger = logger;
            _config = config;
            _getAggregatedDataUseCase = getAggregatedDataUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAggregated(
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] string? author,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                string newsUrl = _config["Apis:NewsApiUrl"]!;
                string redditUrl = _config["Apis:RedditApiUrl"]!;
                string weatherUrl = _config["Apis:WeatherApiUrl"]!;

                var query = new AggregatedQuery
                {
                    SortBy = sortBy,
                    SortDirection = sortDirection,
                    Author = author,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var result = await _getAggregatedDataUseCase.Execute(
                    newsUrl,
                    redditUrl,
                    weatherUrl,
                    query
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting aggregated data.");
                return StatusCode(500, new { message = ex.ToString() });
            }
        }
    }
}
