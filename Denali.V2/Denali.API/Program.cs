using Denali.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
ContainerConfiguration.RegisterServices(builder.Configuration, builder.Environment, builder.Services);

builder.Services.AddCors(options =>
{
    // Allow requests from local files without need for web server
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
