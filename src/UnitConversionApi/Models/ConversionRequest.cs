using System.ComponentModel.DataAnnotations;

namespace UnitConversionApi.Models;

/// <summary>
/// Represents a unit conversion request bound from query parameters.
/// </summary>
public class ConversionRequest
{
    /// <summary>
    /// The numerical value to convert.
    /// </summary>
    /// <example>100</example>
    [Required]
    public double Value { get; set; }

    /// <summary>
    /// The source unit (e.g., "meter", "celsius", "kilogram").
    /// </summary>
    /// <example>meter</example>
    [Required]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// The target unit (e.g., "foot", "fahrenheit", "pound").
    /// </summary>
    /// <example>foot</example>
    [Required]
    public string To { get; set; } = string.Empty;
}
