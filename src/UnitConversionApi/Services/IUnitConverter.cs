namespace UnitConversionApi.Services;

/// <summary>
/// Defines the contract for a unit converter that handles conversions
/// within a specific measurement category (e.g., length, temperature).
/// </summary>
public interface IUnitConverter
{
    /// <summary>
    /// The measurement category this converter handles (e.g., "length", "temperature").
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Returns all unit names supported by this converter.
    /// </summary>
    IReadOnlyList<string> SupportedUnits { get; }

    /// <summary>
    /// Determines whether this converter can handle the specified unit.
    /// </summary>
    bool CanConvert(string unit);

    /// <summary>
    /// Converts a value from one unit to another within this category.
    /// </summary>
    /// <param name="value">The numerical value to convert.</param>
    /// <param name="fromUnit">The source unit.</param>
    /// <param name="toUnit">The target unit.</param>
    /// <returns>The converted value.</returns>
    /// <exception cref="ArgumentException">Thrown when either unit is not supported.</exception>
    double Convert(double value, string fromUnit, string toUnit);
}
