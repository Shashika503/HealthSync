using Microsoft.AspNetCore.Mvc;
using AggregatorService.Jobs;

[ApiController]
[Route("api/test-aggregation")]
public class AggregationTestController : ControllerBase
{
    private readonly AggregationJob _aggregationJob;

    public AggregationTestController(AggregationJob aggregationJob)
    {
        _aggregationJob = aggregationJob;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteAggregation()
    {
        await _aggregationJob.Run();
        return Ok("Aggregation job executed successfully.");
    }
}
