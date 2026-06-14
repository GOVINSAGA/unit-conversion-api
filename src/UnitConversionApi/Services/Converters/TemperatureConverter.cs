namespace UnitConversionApi.Services.Converters;

/// <summary>
/// Converts between temperature units using formula-based conversion.
/// Temperature conversions are non-linear (cannot use simple factors).
/// </summary>
public class TemperatureConverter : IUnitConverter
{
    private static readonly HashSet<string> Units = new(StringComparer.OrdinalIgnoreCase)
    {
        "celsius", "fahrenheit", "kelvin"
    };

    public string Category => "temperature";

    public IReadOnlyList<string> SupportedUnits => Units.ToList().AsReadOnly();

    public bool CanConvert(string unit) => Units.Contains(unit.ToLowerInvariant());

    public double Convert(double value, string fromUnit, string toUnit)
    {
        var from = fromUnit.ToLowerInvariant();
        var to = toUnit.ToLowerInvariant();

        if (!Units.Contains(from))
            throw new ArgumentException($"Unsupported temperature unit: '{fromUnit}'.", nameof(fromUnit));

        if (!Units.Contains(to))
            throw new ArgumentException($"Unsupported temperature unit: '{toUnit}'.", nameof(toUnit));

        if (from == to)
            return value;

        // Convert to Celsius first (intermediate), then to target
        var celsius = from switch
        {
            "celsius" => value,
            "fahrenheit" => (value - 32.0) * 5.0 / 9.0,
            "kelvin" => value - 273.15,
            _ => throw new ArgumentException($"Unsupported temperature unit: '{fromUnit}'.", nameof(fromUnit))
        };

        return to switch
        {
            "celsius" => celsius,
            "fahrenheit" => celsius * 9.0 / 5.0 + 32.0,
            "kelvin" => celsius + 273.15,
            _ => throw new ArgumentException($"Unsupported temperature unit: '{toUnit}'.", nameof(toUnit))
        };
    }
}
