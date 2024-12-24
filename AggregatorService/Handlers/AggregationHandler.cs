using MongoDB.Driver;
using AggregatorService.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AggregatorService.Handlers
{
    public class AggregationHandler
    {
        private readonly IMongoCollection<Appointment> _appointments;
        private readonly Serilog.ILogger _logger;

        public AggregationHandler(IMongoDatabase database , IConfiguration config)
        {
            var collectionName = config.GetSection("DatabaseSettings:UtilizedOtherCollectionName").Value;
            _appointments = database.GetCollection<Appointment>(collectionName);
            _logger = Log.ForContext<AggregationHandler>(); // Initialize Serilog for this class
        }

        // Aggregate number of appointments per doctor
        public async Task<List<DoctorInsight>> GetAppointmentsPerDoctorAsync()
        {
            _logger.Information("Starting aggregation: Number of appointments per doctor.");

            var result = await _appointments
                .Aggregate()
                .Group(
                    a => new { a.DoctorId, a.DoctorName }, // Group by DoctorId and DoctorName
                    g => new DoctorInsight
                    {
                        DoctorId = g.Key.DoctorId,
                        DoctorName = g.Key.DoctorName,
                        AppointmentCount = g.Count()
                    })
                .ToListAsync();

            _logger.Information("Completed aggregation: Number of appointments per doctor. Result: {@DoctorInsights}", result);
            return result;
        }

        // Aggregate frequency of appointments over time
        public async Task<List<AppointmentFrequency>> GetAppointmentFrequencyAsync()
        {
            _logger.Information("Starting aggregation: Appointment frequency over time.");

            var rawResult = await _appointments
                .Aggregate()
                .Group(
                    a => new { a.AppointmentDate.Year, a.AppointmentDate.Month }, // Group by Year and Month
                    g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        AppointmentCount = g.Count()
                    }
                )
                .ToListAsync();

            var result = rawResult.Select(r => new AppointmentFrequency
            {
                Period = $"{r.Year}-{r.Month:D2}", // Format as "yyyy-MM"
                AppointmentCount = r.AppointmentCount
            }).ToList();

            _logger.Information("Completed aggregation: Appointment frequency over time. Result: {@AppointmentFrequency}", result);
            return result;
        }

        // Aggregate common conditions by specialty
        public async Task<List<SpecialtyInsight>> GetCommonConditionsBySpecialtyAsync()
        {
            _logger.Information("Starting aggregation: Common conditions by specialty.");

            var result = await _appointments
                .Aggregate()
                .Group(a => a.Specialty, g => new SpecialtyInsight
                {
                    Specialty = g.Key,
                    CommonConditions = g.Select(a => a.Reason).Distinct().Take(5).ToArray()
                })
                .ToListAsync();

            _logger.Information("Completed aggregation: Common conditions by specialty. Result: {@SpecialtyInsights}", result);
            return result;
        }
    }
}
