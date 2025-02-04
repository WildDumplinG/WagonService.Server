using Server.Services.Settings;
using WagonService.Server.Services;

var builder = WebApplication.CreateBuilder(args);

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