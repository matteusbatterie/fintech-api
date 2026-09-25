using DotNetEnv;
using FinTech.API;
using FinTech.API.Endpoints;
using FinTech.Application;
using FinTech.Infrastructure;
using Scalar.AspNetCore;

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

// Register Application services
builder.Services.AddApplication();

// Register Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Standard API services
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Generates the openapi.json file
    app.MapOpenApi();

    // Modern UI alternative to Swagger (available at /scalar/v1)
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.MapAccountEndpoints();
app.MapTransactionEndpoints();
app.Run();


public partial class Program { }
