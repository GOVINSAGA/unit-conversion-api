namespace UnitConversionApi.Services.Converters;

/// <summary>
/// Converts between weight/mass units.
/// Base unit: kilogram.
/// </summary>
public class WeightConverter : FactorBasedConverter
{
    public WeightConverter() : base(new Dictionary<string, double>
    {
        ["kilogram"] = 1.0,
        ["gram"] = 0.001,
        ["milligram"] = 0.000_001,
        ["pound"] = 0.453_592_37,
        ["ounce"] = 0.028_349_523_125,
        ["ton"] = 1_000.0
    })
    { }

    public override string Category => "weight";
}
