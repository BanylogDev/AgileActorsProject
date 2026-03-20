using AAP.Application.UseCases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/statistics")]
//[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IGetStatisticsUseCase _useCase;

    public StatisticsController(IGetStatisticsUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    public IActionResult GetStatistics()
    {
        var result = _useCase.Execute();
        return Ok(result);
    }
}