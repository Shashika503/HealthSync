using AppointmentSchedulingService.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PatientRecordService.Handlers.CommandHandlers;
using PatientRecordService.Handlers.QueryHandlers;
using PatientRecordService.Validations;
using PatientRecordService.Models;
using PatientRecordService.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddTransient<IValidator<AddPatientCommand>, AddPatientValidator>();
builder.Services.AddTransient<IValidator<UpdatePatientCommand>, UpdatePatientValidator>();
builder.Services.AddTransient<IValidator<DeletePatientCommand>, DeletePatientValidator>();

builder.Services.AddControllers();




// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MongoDB settings from appsettings.json
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("DatabaseSettings"));


// Register MongoClient as a singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

    if (string.IsNullOrEmpty(mongoDbSettings.ConnectionString))
    {
        throw new ArgumentNullException("MongoDB connection string cannot be null.");
    }

    return new MongoClient(mongoDbSettings.ConnectionString);
});

// Register IMongoDatabase scoped to each request
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var mongoDbSettings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();

    if (string.IsNullOrEmpty(mongoDbSettings.DatabaseName))
    {
        throw new ArgumentNullException("MongoDB database name cannot be null.");
    }

    return client.GetDatabase(mongoDbSettings.DatabaseName);
});

// Register command and query handlers for dependency injection
builder.Services.AddScoped<AddPatientHandler>();
builder.Services.AddScoped<GetAllPatientsHandler>();
builder.Services.AddScoped<GetPatientByIdHandler>();
builder.Services.AddScoped<UpdatePatientHandler>();
builder.Services.AddScoped<DeletePatientHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
