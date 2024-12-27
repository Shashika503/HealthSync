using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AppointmentSchedulingService.Models;
using AppointmentSchedulingService.Handlers.CommandHandlers;
using AppointmentSchedulingService.Handlers.QueryHandlers;
using AppointmentSchedulingService.Validations;
using FluentValidation.AspNetCore;
using FluentValidation;
using AppointmentSchedulingService.Commands;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());


builder.Services.AddTransient<IValidator<BookAppointmentCommand>, AddAppointmentValidator>();
builder.Services.AddTransient<IValidator<UpdateAppointmentCommand>, UpdateAppointmentValidator>();
builder.Services.AddTransient<IValidator<CancelAppointmentCommand>, CancelAppointmentValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("DatabaseSettings")
);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

    if (string.IsNullOrEmpty(settings.ConnectionString))
    {
        throw new ArgumentException("MongoDB ConnectionString is missing in appsettings.json.");
    }

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();

    if (string.IsNullOrEmpty(settings.DatabaseName))
    {
        throw new ArgumentException("MongoDB DatabaseName is missing in appsettings.json.");
    }

    return client.GetDatabase(settings.DatabaseName);
});

// Register your services/handlers (e.g., command and query handlers)
builder.Services.AddScoped<BookAppointmentHandler>();
builder.Services.AddScoped<UpdateAppointmentHandler>();
builder.Services.AddScoped<CancelAppointmentHandler>();
builder.Services.AddScoped<GetAppointmentsByDoctorHandler>();
builder.Services.AddScoped<GetAppointmentsByDateHandler>();
builder.Services.AddHttpClient(); // Enables HTTP client injection


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AppointmentSchedulingService API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
