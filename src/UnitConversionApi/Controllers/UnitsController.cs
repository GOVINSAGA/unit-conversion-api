using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

/// <summary>
/// Provides discovery of supported units and categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UnitsController : ControllerBase
{
    private readonly UnitConverterRegistry _registry;

    public UnitsController(UnitConverterRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// Returns all supported units grouped by measurement category.
    /// </summary>
    /// <returns>A list of categories with their supported units.</returns>
    /// <response code="200">Successfully retrieved all supported units.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<UnitCategoryInfo>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var categories = _registry.GetAllConverters()
            .Select(c => new UnitCategoryInfo
            {
                Category = c.Category,
                Units = c.SupportedUnits.ToList()
            })
            .OrderBy(c => c.Category)
            .ToList();

        return Ok(categories);
    }

    /// <summary>
    /// Returns supported units for a specific measurement category.
    /// </summary>
    /// <param name="category">The measurement category (e.g., "length", "temperature").</param>
    /// <returns>The category with its supported units.</returns>
    /// <response code="200">Successfully retrieved units for the category.</response>
    /// <response code="404">Category not found.</response>
    [HttpGet("{category}")]
    [ProducesResponseType(typeof(UnitCategoryInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetByCategory(string category)
    {
        var converter = _registry.GetConverterByCategory(category);

        if (converter is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Category Not Found",
                Detail = $"No conversion category named '{category}' exists.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(new UnitCategoryInfo
        {
            Category = converter.Category,
            Units = converter.SupportedUnits.ToList()
        });
    }
}
