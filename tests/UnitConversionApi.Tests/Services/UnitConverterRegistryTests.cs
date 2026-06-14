using FluentAssertions;
using UnitConversionApi.Services;
using UnitConversionApi.Services.Converters;

namespace UnitConversionApi.Tests.Services;

public class UnitConverterRegistryTests
{
    private readonly UnitConverterRegistry _registry;

    public UnitConverterRegistryTests()
    {
        var converters = new IUnitConverter[]
        {
            new LengthConverter(),
            new TemperatureConverter(),
            new WeightConverter(),
            new VolumeConverter(),
            new SpeedConverter()
        };
        _registry = new UnitConverterRegistry(converters);
    }

    [Fact]
    public void GetAllConverters_ShouldReturnAllRegistered()
    {
        _registry.GetAllConverters().Should().HaveCount(5);
    }

    [Theory]
    [InlineData("meter", "length")]
    [InlineData("celsius", "temperature")]
    [InlineData("kilogram", "weight")]
    [InlineData("liter", "volume")]
    [InlineData("knot", "speed")]
    public void FindConverterForUnit_ShouldReturnCorrectCategory(string unit, string expectedCategory)
    {
        var converter = _registry.FindConverterForUnit(unit);
        converter.Should().NotBeNull();
        converter!.Category.Should().Be(expectedCategory);
    }

    [Fact]
    public void FindConverterForUnit_UnknownUnit_ShouldReturnNull()
    {
        _registry.FindConverterForUnit("parsec").Should().BeNull();
    }

    [Theory]
    [InlineData("length")]
    [InlineData("temperature")]
    [InlineData("weight")]
    [InlineData("volume")]
    [InlineData("speed")]
    public void GetConverterByCategory_ShouldReturnConverter(string category)
    {
        _registry.GetConverterByCategory(category).Should().NotBeNull();
    }

    [Fact]
    public void GetConverterByCategory_UnknownCategory_ShouldReturnNull()
    {
        _registry.GetConverterByCategory("energy").Should().BeNull();
    }

    [Fact]
    public void Convert_ValidConversion_ShouldReturnResult()
    {
        var (result, category) = _registry.Convert(100, "meter", "foot");
        result.Should().BeApproximately(328.084, 0.01);
        category.Should().Be("length");
    }

    [Fact]
    public void Convert_UnknownFromUnit_ShouldThrow()
    {
        var act = () => _registry.Convert(1, "parsec", "meter");
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Unknown unit*parsec*");
    }

    [Fact]
    public void Convert_UnknownToUnit_ShouldThrow()
    {
        var act = () => _registry.Convert(1, "meter", "parsec");
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Unknown unit*parsec*");
    }

    [Fact]
    public void Convert_IncompatibleUnits_ShouldThrow()
    {
        var act = () => _registry.Convert(1, "meter", "celsius");
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Incompatible units*");
    }
}
