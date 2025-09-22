using Denali.Processors.DenaliClimbStrategy;
using Denali.Services;
using Meta.Numerics.Statistics;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<AlpacaService>();
builder.Services.AddSingleton<DataLayerComponent>();
builder.Services.AddScoped<GapUpStreamer>();
builder.Services.AddOptions<DenaliClimbStrategySettings>()
    .Bind(builder.Configuration.GetSection(DenaliClimbStrategySettings.Settings));

builder.Services.AddCors(options =>
{
    options.AddPolicy("localFilePolicy", policy =>
    {
        policy.WithOrigins("null")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("localFilePolicy");

app.Run();
