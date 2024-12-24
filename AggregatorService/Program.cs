using AggregatorService.Handlers;
using AggregatorService.Jobs;
using AggregatorService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return client.GetDatabase(settings.DatabaseName);
});

// Register Aggregation Services
builder.Services.AddScoped<AggregationHandler>();
builder.Services.AddScoped<AggregationJob>();

// Add Hosted Service for Aggregation Job
builder.Services.AddHostedService<AggregationHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// AggregationHostedService Implementation
public class AggregationHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly bool _isTestMode;

    public AggregationHostedService(IServiceProvider serviceProvider, bool isTestMode = false)
    {
        _serviceProvider = serviceProvider;
        _isTestMode = isTestMode;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var aggregationJob = scope.ServiceProvider.GetRequiredService<AggregationJob>();
                await aggregationJob.Run(); // Execute the aggregation job
            }

            if (_isTestMode) break; // Exit the loop for testing

            // Wait 24 hours before the next execution
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
    }


}
