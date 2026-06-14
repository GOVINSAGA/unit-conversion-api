namespace UnitConversionApi.Services.Converters;

/// <summary>
/// Base class for converters that use a factor-based approach.
/// Each unit has a factor relative to a base unit. To convert from unit A to unit B:
/// result = value * (factorA / factorB)
/// </summary>
public abstract class FactorBasedConverter : IUnitConverter
{
    private readonly Dictionary<string, double> _factors;

    protected FactorBasedConverter(Dictionary<string, double> factors)
    {
        _factors = factors;
    }

    public abstract string Category { get; }

    public IReadOnlyList<string> SupportedUnits => _factors.Keys.ToList().AsReadOnly();

    public bool CanConvert(string unit) => _factors.ContainsKey(unit.ToLowerInvariant());

    public double Convert(double value, string fromUnit, string toUnit)
    {
        var from = fromUnit.ToLowerInvariant();
        var to = toUnit.ToLowerInvariant();

        if (!_factors.TryGetValue(from, out var fromFactor))
            throw new ArgumentException($"Unsupported {Category} unit: '{fromUnit}'.", nameof(fromUnit));

        if (!_factors.TryGetValue(to, out var toFactor))
            throw new ArgumentException($"Unsupported {Category} unit: '{toUnit}'.", nameof(toUnit));

        // Convert: source → base unit → target unit
        return value * (fromFactor / toFactor);
    }
}
