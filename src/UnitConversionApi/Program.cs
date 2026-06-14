using Scalar.AspNetCore;
using UnitConversionApi.Middleware;
using UnitConversionApi.Services;
using UnitConversionApi.Services.Converters;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Service Registration
// ---------------------------------------------------------------------------

// Register individual converters
builder.Services.AddSingleton<IUnitConverter, LengthConverter>();
builder.Services.AddSingleton<IUnitConverter, TemperatureConverter>();
builder.Services.AddSingleton<IUnitConverter, WeightConverter>();
builder.Services.AddSingleton<IUnitConverter, VolumeConverter>();
builder.Services.AddSingleton<IUnitConverter, SpeedConverter>();

// Register the converter registry (aggregates all IUnitConverter instances)
builder.Services.AddSingleton<UnitConverterRegistry>();

// Controllers
builder.Services.AddControllers();

// OpenAPI (native .NET 10 support)
builder.Services.AddOpenApi();

// CORS — allow all origins for development / demo purposes
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// RFC 7807 ProblemDetails
builder.Services.AddProblemDetails();

var app = builder.Build();

// ---------------------------------------------------------------------------
// Middleware Pipeline
// ---------------------------------------------------------------------------

app.UseMiddleware<ExceptionHandlingMiddleware>();

// OpenAPI document endpoint
app.MapOpenApi();

// Scalar API reference UI (modern replacement for Swagger UI)
app.MapScalarApiReference(options =>
{
    options.WithTitle("Unit Conversion API")
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make the Program class accessible for integration tests
// See: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
public partial class Program { }
