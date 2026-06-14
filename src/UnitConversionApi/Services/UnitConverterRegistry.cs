namespace UnitConversionApi.Services;

/// <summary>
/// Aggregates all registered <see cref="IUnitConverter"/> implementations
/// and dispatches conversion requests to the appropriate converter.
/// </summary>
public class UnitConverterRegistry
{
    private readonly IReadOnlyList<IUnitConverter> _converters;

    public UnitConverterRegistry(IEnumerable<IUnitConverter> converters)
    {
        _converters = converters.ToList().AsReadOnly();
    }

    /// <summary>
    /// Finds the converter that supports the given unit.
    /// </summary>
    public IUnitConverter? FindConverterForUnit(string unit)
    {
        var normalizedUnit = unit.Trim().ToLowerInvariant();
        return _converters.FirstOrDefault(c => c.CanConvert(normalizedUnit));
    }

    /// <summary>
    /// Returns all registered converters.
    /// </summary>
    public IReadOnlyList<IUnitConverter> GetAllConverters() => _converters;

    /// <summary>
    /// Returns the converter for a specific category, or null if not found.
    /// </summary>
    public IUnitConverter? GetConverterByCategory(string category)
    {
        var normalizedCategory = category.Trim().ToLowerInvariant();
        return _converters.FirstOrDefault(c =>
            c.Category.Equals(normalizedCategory, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Converts a value between two units, automatically finding the correct converter.
    /// </summary>
    /// <returns>A tuple of (result, category).</returns>
    /// <exception cref="ArgumentException">Thrown when units are unknown or incompatible.</exception>
    public (double Result, string Category) Convert(double value, string fromUnit, string toUnit)
    {
        var normalizedFrom = fromUnit.Trim().ToLowerInvariant();
        var normalizedTo = toUnit.Trim().ToLowerInvariant();

        var fromConverter = FindConverterForUnit(normalizedFrom);
        var toConverter = FindConverterForUnit(normalizedTo);

        if (fromConverter is null)
            throw new ArgumentException($"Unknown unit: '{fromUnit}'.", nameof(fromUnit));

        if (toConverter is null)
            throw new ArgumentException($"Unknown unit: '{toUnit}'.", nameof(toUnit));

        if (fromConverter.Category != toConverter.Category)
        {
            throw new ArgumentException(
                $"Incompatible units: '{fromUnit}' ({fromConverter.Category}) cannot be converted to '{toUnit}' ({toConverter.Category}). " +
                $"Both units must belong to the same measurement category.");
        }

        var result = fromConverter.Convert(value, normalizedFrom, normalizedTo);
        return (result, fromConverter.Category);
    }
}
