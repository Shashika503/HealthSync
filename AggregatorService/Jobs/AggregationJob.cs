using MongoDB.Driver;
using AggregatorService.Models;
using AggregatorService.Handlers;
using System.Threading.Tasks;

namespace AggregatorService.Jobs
{
    public class AggregationJob
    {
        private readonly AggregationHandler _aggregationHandler;
        private readonly IMongoCollection<AggregatedInsight> _insightsCollection;

        public AggregationJob(IMongoDatabase database, IConfiguration config, AggregationHandler aggregationHandler)
        {
            _aggregationHandler = aggregationHandler;
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _insightsCollection = database.GetCollection<AggregatedInsight>(collectionName);
        }

        public async Task Run()
        {
            var doctorInsights = await _aggregationHandler.GetAppointmentsPerDoctorAsync();
            var appointmentFrequency = await _aggregationHandler.GetAppointmentFrequencyAsync();
            var conditionsBySpecialty = await _aggregationHandler.GetCommonConditionsBySpecialtyAsync();

            var aggregatedInsight = new AggregatedInsight
            {
                DoctorInsights = doctorInsights.ToArray(),
                AppointmentFrequency = appointmentFrequency.ToArray(),
                ConditionsBySpecialty = conditionsBySpecialty.ToArray()
            };

            await _insightsCollection.InsertOneAsync(aggregatedInsight);
            Console.WriteLine("Aggregation job completed successfully.");
        }

    }
}
