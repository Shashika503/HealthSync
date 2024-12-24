using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var builder = WebApplication.CreateBuilder(args);
//AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

//builder.Services.AddHttpClient("PatientsService", c =>
//{
//    c.BaseAddress = new Uri("https://localhost:7287");
//}).ConfigurePrimaryHttpMessageHandler(() =>
//{
//    return new HttpClientHandler
//    {
//        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//    };
//});

// Add Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);



// Add Ocelot and required services
builder.Services.AddOcelot();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var app = builder.Build();


app.UseRouting();
app.UseAuthorization();


// Use Ocelot Middleware
await app.UseOcelot();

app.Run();
