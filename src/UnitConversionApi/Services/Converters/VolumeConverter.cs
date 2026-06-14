namespace UnitConversionApi.Services.Converters;

/// <summary>
/// Converts between volume units.
/// Base unit: liter.
/// </summary>
public class VolumeConverter : FactorBasedConverter
{
    public VolumeConverter() : base(new Dictionary<string, double>
    {
        ["liter"] = 1.0,
        ["milliliter"] = 0.001,
        ["gallon"] = 3.785_411_784,
        ["quart"] = 0.946_352_946,
        ["pint"] = 0.473_176_473,
        ["cup"] = 0.236_588_236_5,
        ["fluid_ounce"] = 0.029_573_529_5625
    })
    { }

    public override string Category => "volume";
}
