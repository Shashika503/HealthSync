using MongoDB.Driver;
using AggregatorService.Models;
using AggregatorService.Handlers;
using Serilog;
using System;
using System.Threading.Tasks;
using Npgsql;

namespace AggregatorService.Jobs
{
    public class AggregationJob
    {
        private readonly AggregationHandler _aggregationHandler;
        private readonly IMongoCollection<AggregatedInsight> _insightsCollection;
        private readonly Serilog.ILogger _logger;
        private readonly NpgsqlConnection _redshiftConnection;

        public AggregationJob(IMongoDatabase database, IConfiguration config, AggregationHandler aggregationHandler, NpgsqlConnection redshiftConnection)
        {
            _aggregationHandler = aggregationHandler;
            _logger = Log.ForContext<AggregationJob>();

            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _insightsCollection = database.GetCollection<AggregatedInsight>(collectionName);

            _redshiftConnection = redshiftConnection;
        }

        public async Task Run()
        {
            _logger.Information("Aggregation job started at {Time}", DateTime.UtcNow);

            try
            {
                // Step 1: Aggregate data
                var doctorInsights = await _aggregationHandler.GetAppointmentsPerDoctorAsync();
                var appointmentFrequency = await _aggregationHandler.GetAppointmentFrequencyAsync();
                var conditionsBySpecialty = await _aggregationHandler.GetCommonConditionsBySpecialtyAsync();

                _logger.Information("Data aggregated: {DoctorCount} doctors, {FrequencyCount} periods, {SpecialtyCount} specialties.",
                    doctorInsights.Count, appointmentFrequency.Count, conditionsBySpecialty.Count);

                // Step 2: Create aggregated insight document
                var aggregatedInsight = new AggregatedInsight
                {
                    DoctorInsights = doctorInsights.ToArray(),
                    AppointmentFrequency = appointmentFrequency.ToArray(),
                    ConditionsBySpecialty = conditionsBySpecialty.ToArray()
                };

                // Step 3: Save aggregated insights to MongoDB
                await _insightsCollection.InsertOneAsync(aggregatedInsight);
                _logger.Information("Aggregated insights saved to MongoDB at {Time}.", DateTime.UtcNow);

                // Step 4: Save aggregated insights to Redshift
                await SaveToRedshift(doctorInsights, appointmentFrequency, conditionsBySpecialty);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while running the aggregation job.");
            }
        }

        private async Task SaveToRedshift(
    IEnumerable<DoctorInsight> doctorInsights,
    IEnumerable<AppointmentFrequency> appointmentFrequency,
    IEnumerable<SpecialtyInsight> conditionsBySpecialty)
        {
            _logger.Information("Saving aggregated insights to Redshift...");

            using (var connection = new NpgsqlConnection(_redshiftConnection.ConnectionString))
            {
                await connection.OpenAsync();

                try
                {
                    // Save Doctor Insights
                    var doctorStagingTable = "doctor_insights_staging";
                    await CreateStagingTableAsync(connection, doctorStagingTable, "doctor_insights");

                    foreach (var insight in doctorInsights)
                    {
                        var insertQuery = $@"
                    INSERT INTO {doctorStagingTable} (doctor_id, doctor_name, appointment_count)
                    VALUES (@DoctorId, @DoctorName, @AppointmentCount)";

                        using (var command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("DoctorId", insight.DoctorId);
                            command.Parameters.AddWithValue("DoctorName", insight.DoctorName);
                            command.Parameters.AddWithValue("AppointmentCount", insight.AppointmentCount);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    await UpsertFromStagingTableAsync(connection, doctorStagingTable, "doctor_insights", "doctor_id");
                    _logger.Information("Doctor insights saved to Redshift.");

                    // Save Appointment Frequency
                    var frequencyStagingTable = "appointment_frequency_staging";
                    await CreateStagingTableAsync(connection, frequencyStagingTable, "appointment_frequency_insights");

                    foreach (var frequency in appointmentFrequency)
                    {
                        var insertQuery = $@"
                    INSERT INTO {frequencyStagingTable} (period, appointment_count)
                    VALUES (@Period, @AppointmentCount)";

                        using (var command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("Period", frequency.Period);
                            command.Parameters.AddWithValue("AppointmentCount", frequency.AppointmentCount);
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    await UpsertFromStagingTableAsync(connection, frequencyStagingTable, "appointment_frequency_insights", "period");
                    _logger.Information("Appointment frequency saved to Redshift.");

                    // Save Specialty Insights
                    var specialtyStagingTable = "specialty_insights_staging";
                    await CreateStagingTableAsync(connection, specialtyStagingTable, "specialty_insights");

                    foreach (var specialty in conditionsBySpecialty)
                    {
                        var insertQuery = $@"
                    INSERT INTO {specialtyStagingTable} (specialty, common_conditions)
                    VALUES (@Specialty, @CommonConditions)";

                        using (var command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("Specialty", specialty.Specialty);
                            command.Parameters.AddWithValue("CommonConditions", string.Join(", ", specialty.CommonConditions));
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    await UpsertFromStagingTableAsync(connection, specialtyStagingTable, "specialty_insights", "specialty");
                    _logger.Information("Specialty insights saved to Redshift.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred while saving data to Redshift.");
                }
            }
        }

        private async Task CreateStagingTableAsync(NpgsqlConnection connection, string stagingTable, string targetTable)
        {
            var createQuery = $@"
        CREATE TEMP TABLE {stagingTable} (LIKE {targetTable});";

            using (var command = new NpgsqlCommand(createQuery, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task UpsertFromStagingTableAsync(NpgsqlConnection connection, string stagingTable, string targetTable, string primaryKey)
        {
            var deleteQuery = $@"
        DELETE FROM {targetTable}
        USING {stagingTable}
        WHERE {targetTable}.{primaryKey} = {stagingTable}.{primaryKey};";

            var insertQuery = $@"
        INSERT INTO {targetTable}
        SELECT * FROM {stagingTable};";

            using (var deleteCommand = new NpgsqlCommand(deleteQuery, connection))
            {
                await deleteCommand.ExecuteNonQueryAsync();
            }

            using (var insertCommand = new NpgsqlCommand(insertQuery, connection))
            {
                await insertCommand.ExecuteNonQueryAsync();
            }
        }



    }
}
