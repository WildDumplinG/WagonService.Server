using WagonService.Server.Services;
using static WagonService.Server.Services.WagonServiceImpl;

var builder = WebApplication.CreateBuilder(args);

#region Config

builder.Services.Configure<WagonServiceImplSettings>(builder.Configuration.GetSection("WagonServiceImplSettings"));

#endregion

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<WagonServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client...");

app.Run();