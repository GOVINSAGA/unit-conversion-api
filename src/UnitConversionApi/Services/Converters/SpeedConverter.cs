namespace UnitConversionApi.Services.Converters;

/// <summary>
/// Converts between speed units.
/// Base unit: meters per second (m/s).
/// </summary>
public class SpeedConverter : FactorBasedConverter
{
    public SpeedConverter() : base(new Dictionary<string, double>
    {
        ["meters_per_second"] = 1.0,
        ["kilometers_per_hour"] = 1.0 / 3.6,
        ["miles_per_hour"] = 0.447_04,
        ["knot"] = 0.514_444
    })
    { }

    public override string Category => "speed";
}
