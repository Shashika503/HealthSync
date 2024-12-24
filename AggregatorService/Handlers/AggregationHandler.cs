using MongoDB.Driver;
using AggregatorService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AggregatorService.Handlers
{
    public class AggregationHandler
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public AggregationHandler(IMongoDatabase database)
        {
            _appointments = database.GetCollection<Appointment>("Appointments_Records");
        }

        // Aggregate number of appointments per doctor
        public async Task<List<DoctorInsight>> GetAppointmentsPerDoctorAsync()
        {
            return await _appointments
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
        }


        public async Task<List<AppointmentFrequency>> GetAppointmentFrequencyAsync()
        {
            var result = await _appointments
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

            // Construct the "yyyy-MM" period after aggregation
            return result.Select(r => new AppointmentFrequency
            {
                Period = $"{r.Year}-{r.Month:D2}", // Format as "yyyy-MM"
                AppointmentCount = r.AppointmentCount
            }).ToList();
        }


        // Aggregate common conditions by specialty
        public async Task<List<SpecialtyInsight>> GetCommonConditionsBySpecialtyAsync()
        {
            return await _appointments
                .Aggregate()
                .Group(a => a.Specialty, g => new SpecialtyInsight
                {
                    Specialty = g.Key,
                    CommonConditions = g.Select(a => a.Reason).Distinct().Take(5).ToArray()
                })
                .ToListAsync();
        }
    }
}
