using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

/// <summary>
/// Provides unit conversion operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConvertController : ControllerBase
{
    private readonly UnitConverterRegistry _registry;
    private readonly ILogger<ConvertController> _logger;

    public ConvertController(UnitConverterRegistry registry, ILogger<ConvertController> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    /// <summary>
    /// Converts a numerical value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion parameters.</param>
    /// <returns>The conversion result including the converted value and category.</returns>
    /// <response code="200">Conversion successful.</response>
    /// <response code="400">Invalid request — unknown unit or incompatible units.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Convert([FromQuery] ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.From))
            return BadRequest(CreateProblem("The 'from' parameter is required."));

        if (string.IsNullOrWhiteSpace(request.To))
            return BadRequest(CreateProblem("The 'to' parameter is required."));

        try
        {
            var (result, category) = _registry.Convert(request.Value, request.From, request.To);

            _logger.LogInformation(
                "Converted {Value} {From} → {To} = {Result} ({Category})",
                request.Value, request.From, request.To, result, category);

            return Ok(new ConversionResponse
            {
                Value = request.Value,
                FromUnit = request.From.Trim().ToLowerInvariant(),
                ToUnit = request.To.Trim().ToLowerInvariant(),
                Result = Math.Round(result, 6),
                Category = category
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Conversion failed for {From} → {To}", request.From, request.To);
            return BadRequest(CreateProblem(ex.Message));
        }
    }

    private static ProblemDetails CreateProblem(string detail) => new()
    {
        Title = "Invalid Conversion Request",
        Detail = detail,
        Status = StatusCodes.Status400BadRequest
    };
}
