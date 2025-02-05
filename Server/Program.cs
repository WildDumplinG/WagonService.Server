using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Server.Services.Settings;
using WagonService.Server.Services;

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

builder.Services.AddOpenTelemetry()
    .WithTracing(tracingBuilder =>
    {
        tracingBuilder
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
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