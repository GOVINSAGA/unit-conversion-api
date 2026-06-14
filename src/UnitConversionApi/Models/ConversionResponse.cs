namespace UnitConversionApi.Models;

/// <summary>
/// Represents the result of a unit conversion.
/// </summary>
public class ConversionResponse
{
    /// <summary>
    /// The original input value.
    /// </summary>
    /// <example>100</example>
    public double Value { get; set; }

    /// <summary>
    /// The source unit.
    /// </summary>
    /// <example>meter</example>
    public string FromUnit { get; set; } = string.Empty;

    /// <summary>
    /// The target unit.
    /// </summary>
    /// <example>foot</example>
    public string ToUnit { get; set; } = string.Empty;

    /// <summary>
    /// The converted result.
    /// </summary>
    /// <example>328.084</example>
    public double Result { get; set; }

    /// <summary>
    /// The measurement category (e.g., "length", "temperature").
    /// </summary>
    /// <example>length</example>
    public string Category { get; set; } = string.Empty;
}
