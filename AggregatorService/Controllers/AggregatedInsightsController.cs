using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using AggregatorService.Models;
using System.Threading.Tasks;

namespace AggregatorService.Controllers
{
    [ApiController]
    [Route("api/aggregated-insights")]
    public class AggregatedInsightsController : ControllerBase
    {
        private readonly IMongoCollection<AggregatedInsight> _insightsCollection;

        public AggregatedInsightsController(IMongoDatabase database, IConfiguration config)
        {
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _insightsCollection = database.GetCollection<AggregatedInsight>(collectionName);
        }

        // Get the latest aggregated insights
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestInsights()
        {
            var latestInsight = await _insightsCollection
                .Find(Builders<AggregatedInsight>.Filter.Empty)
                .SortByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            if (latestInsight == null)
            {
                return NotFound("No aggregated insights available.");
            }

            return Ok(latestInsight);
        }

        // Get appointments per doctor
        [HttpGet("doctors")]
        public async Task<IActionResult> GetAppointmentsPerDoctor()
        {
            var latestInsight = await _insightsCollection
                .Find(Builders<AggregatedInsight>.Filter.Empty)
                .SortByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            if (latestInsight == null)
            {
                return NotFound("No doctor insights available.");
            }

            return Ok(latestInsight.DoctorInsights);
        }

        // Get appointment frequency over time
        [HttpGet("frequency")]
        public async Task<IActionResult> GetAppointmentFrequency()
        {
            var latestInsight = await _insightsCollection
                .Find(Builders<AggregatedInsight>.Filter.Empty)
                .SortByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            if (latestInsight == null)
            {
                return NotFound("No appointment frequency insights available.");
            }

            return Ok(latestInsight.AppointmentFrequency);
        }

        // Get common conditions by specialty
        [HttpGet("conditions")]
        public async Task<IActionResult> GetConditionsBySpecialty()
        {
            var latestInsight = await _insightsCollection
                .Find(Builders<AggregatedInsight>.Filter.Empty)
                .SortByDescending(i => i.Id)
                .FirstOrDefaultAsync();

            if (latestInsight == null)
            {
                return NotFound("No specialty insights available.");
            }

            return Ok(latestInsight.ConditionsBySpecialty);
        }
    }
}
