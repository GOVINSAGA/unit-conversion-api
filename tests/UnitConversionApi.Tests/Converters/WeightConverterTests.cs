using FluentAssertions;
using UnitConversionApi.Services.Converters;

namespace UnitConversionApi.Tests.Converters;

public class WeightConverterTests
{
    private readonly WeightConverter _converter = new();

    [Fact]
    public void Category_ShouldBeWeight()
    {
        _converter.Category.Should().Be("weight");
    }

    [Fact]
    public void SupportedUnits_ShouldContainExpectedUnits()
    {
        _converter.SupportedUnits.Should().Contain(new[]
        {
            "kilogram", "gram", "milligram", "pound", "ounce", "ton"
        });
    }

    [Theory]
    [InlineData(1.0, "kilogram", "gram", 1000.0)]
    [InlineData(1000.0, "gram", "kilogram", 1.0)]
    [InlineData(1.0, "kilogram", "pound", 2.20462)]
    [InlineData(1.0, "pound", "ounce", 16.0)]
    [InlineData(1.0, "ton", "kilogram", 1000.0)]
    [InlineData(1.0, "kilogram", "milligram", 1_000_000.0)]
    public void Convert_KnownConversions_ShouldBeAccurate(
        double value, string from, string to, double expected)
    {
        var result = _converter.Convert(value, from, to);
        result.Should().BeApproximately(expected, 0.01);
    }

    [Fact]
    public void Convert_Zero_ShouldReturnZero()
    {
        _converter.Convert(0.0, "kilogram", "pound").Should().Be(0.0);
    }

    [Fact]
    public void Convert_Bidirectional_ShouldBeConsistent()
    {
        var kg = 5.0;
        var pounds = _converter.Convert(kg, "kilogram", "pound");
        var backToKg = _converter.Convert(pounds, "pound", "kilogram");
        backToKg.Should().BeApproximately(kg, 0.0001);
    }
}
