using MongoDB.Driver;
using AggregatorService.Models;
using AggregatorService.Handlers;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AggregatorService.Jobs
{
    public class AggregationJob
    {
        private readonly AggregationHandler _aggregationHandler;
        private readonly IMongoCollection<AggregatedInsight> _insightsCollection;
        private readonly Serilog.ILogger _logger;

        public AggregationJob(IMongoDatabase database, IConfiguration config, AggregationHandler aggregationHandler)
        {
            _aggregationHandler = aggregationHandler;
            _logger = Log.ForContext<AggregationJob>(); // Create a Serilog context for this class

            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _insightsCollection = database.GetCollection<AggregatedInsight>(collectionName);
        }

        public async Task Run()
        {
            _logger.Information("Aggregation job started at {Time}", DateTime.UtcNow);

            try
            {
                // Aggregate data
                var doctorInsights = await _aggregationHandler.GetAppointmentsPerDoctorAsync();
                _logger.Information("Aggregated {Count} doctor insights.", doctorInsights.Count);

                var appointmentFrequency = await _aggregationHandler.GetAppointmentFrequencyAsync();
                _logger.Information("Aggregated appointment frequency for {Count} periods.", appointmentFrequency.Count);

                var conditionsBySpecialty = await _aggregationHandler.GetCommonConditionsBySpecialtyAsync();
                _logger.Information("Aggregated insights for {Count} specialties.", conditionsBySpecialty.Count);

                // Create aggregated insight document
                var aggregatedInsight = new AggregatedInsight
                {
                    DoctorInsights = doctorInsights.ToArray(),
                    AppointmentFrequency = appointmentFrequency.ToArray(),
                    ConditionsBySpecialty = conditionsBySpecialty.ToArray()
                };

                // Log data before saving
                _logger.Information("Saving aggregated insights into MongoDB...");

                // Save to MongoDB
                await _insightsCollection.InsertOneAsync(aggregatedInsight);

                // Log success
                _logger.Information("Aggregated insights successfully saved to MongoDB at {Time}.", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                // Log any errors
                _logger.Error(ex, "An error occurred while running the aggregation job.");
            }
        }
    }
}
