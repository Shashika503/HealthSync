using AggregatorService.Handlers;
using AggregatorService.Jobs;
using AggregatorService.Models;
using AggregatorService.Services;
using AggregatorService.Validations;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Logs to the console
    .WriteTo.File("logs/aggregator-service.log", rollingInterval: RollingInterval.Day) // Logs to a file
    .CreateLogger();

// Add Serilog as the logging provider
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());


builder.Services.AddTransient<IValidator<AggregatedInsight>, AggregatedInsightValidator>();
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
