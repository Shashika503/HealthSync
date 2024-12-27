using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Handlers.CommandHandlers;
using NotificationService.Models;
using NotificationService.Services;
using NotificationService.Validations;
using NotificationService.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());


builder.Services.AddTransient<IValidator<SendReminderCommand>, NotificationValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<NotificationSender>();


// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Add Services
builder.Services.AddSingleton<NotificationSender>();
builder.Services.AddScoped<SendReminderHandler>();
builder.Services.AddHostedService<NotificationBackgroundService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
