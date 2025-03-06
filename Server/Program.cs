using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Server.Services.Settings;
using WagonService.Server.Services;
using Serilog;
using Serilog.Sinks.Loki;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

var type = System.Environment.GetEnvironmentVariable("env");

if (type != null)
{
    try
    {
        builder.Configuration.AddJsonFile($"appsettings.{type}.json",
            optional: true,
            reloadOnChange: true
            );


    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

#region Logger

Log.Logger = new LoggerConfiguration()
    .WriteTo.Http("http://localhost:3100/loki/api/v1/push")
    .WriteTo.LokiHttp("http://localhost:3100")
    .CreateLogger();


Log.ForContext("job", "app")
   .Information("Starting up the web API.");

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

#endregion

builder.Services.AddOpenTelemetry()
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "localhost";
                options.AgentPort = 6831;
            })
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Jaeger:WagonServer"));
    })
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter()
            .AddPrometheusExporter(options => 
            { 
                
            });
    });

#region Config

builder.Services.Configure<WagonServiceImplSettings>(builder.Configuration.GetSection("WagonServiceImplSettings"));

#endregion

builder.WebHost.UseKestrel();

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<WagonServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client...");

app.Run();