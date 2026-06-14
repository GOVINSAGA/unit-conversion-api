namespace UnitConversionApi.Services.Converters;

/// <summary>
/// Converts between length/distance units.
/// Base unit: meter.
/// </summary>
public class LengthConverter : FactorBasedConverter
{
    public LengthConverter() : base(new Dictionary<string, double>
    {
        ["meter"] = 1.0,
        ["kilometer"] = 1_000.0,
        ["centimeter"] = 0.01,
        ["millimeter"] = 0.001,
        ["mile"] = 1_609.344,
        ["yard"] = 0.9144,
        ["foot"] = 0.3048,
        ["inch"] = 0.0254
    })
    { }

    public override string Category => "length";
}
