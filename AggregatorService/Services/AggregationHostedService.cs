using AggregatorService.Jobs;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AggregatorService.Services
{
    public class AggregationHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Serilog.ILogger _logger;

        public AggregationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = Log.ForContext<AggregationHostedService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var aggregationJob = scope.ServiceProvider.GetRequiredService<AggregationJob>();
                        _logger.Information("Starting aggregation job at {Time}", DateTime.UtcNow);
                        await aggregationJob.Run();
                    }

                    _logger.Information("Aggregation job completed. Next run scheduled after 24 hours.");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred while running the AggregationHostedService.");
                }

                // Wait 24 hours before the next execution
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }


}
