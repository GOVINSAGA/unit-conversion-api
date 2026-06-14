using FluentAssertions;
using UnitConversionApi.Services.Converters;

namespace UnitConversionApi.Tests.Converters;

public class TemperatureConverterTests
{
    private readonly TemperatureConverter _converter = new();

    [Fact]
    public void Category_ShouldBeTemperature()
    {
        _converter.Category.Should().Be("temperature");
    }

    [Fact]
    public void SupportedUnits_ShouldContainExpectedUnits()
    {
        _converter.SupportedUnits.Should().Contain(new[]
        {
            "celsius", "fahrenheit", "kelvin"
        });
    }

    [Fact]
    public void Convert_SameUnit_ShouldReturnSameValue()
    {
        _converter.Convert(100.0, "celsius", "celsius").Should().Be(100.0);
    }

    // Water freezing point
    [Theory]
    [InlineData(0.0, "celsius", "fahrenheit", 32.0)]
    [InlineData(32.0, "fahrenheit", "celsius", 0.0)]
    [InlineData(0.0, "celsius", "kelvin", 273.15)]
    [InlineData(273.15, "kelvin", "celsius", 0.0)]
    public void Convert_FreezingPoint_ShouldBeAccurate(
        double value, string from, string to, double expected)
    {
        var result = _converter.Convert(value, from, to);
        result.Should().BeApproximately(expected, 0.0001);
    }

    // Water boiling point
    [Theory]
    [InlineData(100.0, "celsius", "fahrenheit", 212.0)]
    [InlineData(212.0, "fahrenheit", "celsius", 100.0)]
    [InlineData(100.0, "celsius", "kelvin", 373.15)]
    [InlineData(373.15, "kelvin", "celsius", 100.0)]
    public void Convert_BoilingPoint_ShouldBeAccurate(
        double value, string from, string to, double expected)
    {
        var result = _converter.Convert(value, from, to);
        result.Should().BeApproximately(expected, 0.0001);
    }

    // Absolute zero
    [Theory]
    [InlineData(0.0, "kelvin", "celsius", -273.15)]
    [InlineData(-273.15, "celsius", "kelvin", 0.0)]
    [InlineData(0.0, "kelvin", "fahrenheit", -459.67)]
    public void Convert_AbsoluteZero_ShouldBeAccurate(
        double value, string from, string to, double expected)
    {
        var result = _converter.Convert(value, from, to);
        result.Should().BeApproximately(expected, 0.01);
    }

    // Body temperature
    [Fact]
    public void Convert_BodyTemperature_ShouldBeAccurate()
    {
        var result = _converter.Convert(98.6, "fahrenheit", "celsius");
        result.Should().BeApproximately(37.0, 0.01);
    }

    [Fact]
    public void Convert_UnsupportedUnit_ShouldThrow()
    {
        var act = () => _converter.Convert(1.0, "celsius", "rankine");
        act.Should().Throw<ArgumentException>();
    }
}
