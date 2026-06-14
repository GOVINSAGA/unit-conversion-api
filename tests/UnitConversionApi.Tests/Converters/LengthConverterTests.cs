using FluentAssertions;
using UnitConversionApi.Services.Converters;

namespace UnitConversionApi.Tests.Converters;

public class LengthConverterTests
{
    private readonly LengthConverter _converter = new();

    [Fact]
    public void Category_ShouldBeLength()
    {
        _converter.Category.Should().Be("length");
    }

    [Fact]
    public void SupportedUnits_ShouldContainExpectedUnits()
    {
        _converter.SupportedUnits.Should().Contain(new[]
        {
            "meter", "kilometer", "centimeter", "millimeter",
            "mile", "yard", "foot", "inch"
        });
    }

    [Theory]
    [InlineData("meter", true)]
    [InlineData("foot", true)]
    [InlineData("lightyear", false)]
    public void CanConvert_ShouldReturnExpectedResult(string unit, bool expected)
    {
        _converter.CanConvert(unit).Should().Be(expected);
    }

    [Fact]
    public void Convert_SameUnit_ShouldReturnSameValue()
    {
        _converter.Convert(42.0, "meter", "meter").Should().Be(42.0);
    }

    [Theory]
    [InlineData(1.0, "kilometer", "meter", 1000.0)]
    [InlineData(1000.0, "meter", "kilometer", 1.0)]
    [InlineData(1.0, "mile", "kilometer", 1.609344)]
    [InlineData(1.0, "foot", "inch", 12.0)]
    [InlineData(1.0, "yard", "foot", 3.0)]
    [InlineData(100.0, "centimeter", "meter", 1.0)]
    public void Convert_KnownConversions_ShouldBeAccurate(
        double value, string from, string to, double expected)
    {
        var result = _converter.Convert(value, from, to);
        result.Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void Convert_Zero_ShouldReturnZero()
    {
        _converter.Convert(0.0, "meter", "foot").Should().Be(0.0);
    }

    [Fact]
    public void Convert_NegativeValue_ShouldWork()
    {
        var result = _converter.Convert(-100.0, "centimeter", "meter");
        result.Should().BeApproximately(-1.0, 0.0001);
    }

    [Fact]
    public void Convert_UnsupportedUnit_ShouldThrow()
    {
        var act = () => _converter.Convert(1.0, "meter", "lightyear");
        act.Should().Throw<ArgumentException>();
    }
}
