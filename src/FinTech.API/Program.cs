using FinTech.Infrastructure;
using Scalar.AspNetCore;
using DotNetEnv;

// Load the .env file first
Env.Load();

// Load and interpolate variables inside the .env file
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add .env file to configuration sources
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("ConnectionString is null! Check if .env is in the root folder.");
}

// Register your Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Standard API services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Generates the openapi.json file
    app.MapOpenApi();

    // Modern UI alternative to Swagger (available at /scalar/v1)
    app.MapScalarApiReference();
}

app.MapControllers();
app.Run();
