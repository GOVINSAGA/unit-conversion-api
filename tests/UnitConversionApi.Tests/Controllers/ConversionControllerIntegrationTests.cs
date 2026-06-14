using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Models;

namespace UnitConversionApi.Tests.Controllers;

public class ConversionControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConversionControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Convert_ValidRequest_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=meter&to=foot");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        result.Should().NotBeNull();
        result!.Value.Should().Be(100);
        result.FromUnit.Should().Be("meter");
        result.ToUnit.Should().Be("foot");
        result.Result.Should().BeApproximately(328.084, 0.01);
        result.Category.Should().Be("length");
    }

    [Fact]
    public async Task Convert_TemperatureConversion_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=celsius&to=fahrenheit");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        result.Should().NotBeNull();
        result!.Result.Should().BeApproximately(212.0, 0.01);
    }

    [Fact]
    public async Task Convert_UnknownUnit_ShouldReturn400()
    {
        var response = await _client.GetAsync("/api/convert?value=1&from=meter&to=lightyear");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_IncompatibleUnits_ShouldReturn400()
    {
        var response = await _client.GetAsync("/api/convert?value=1&from=meter&to=celsius");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_MissingFromParam_ShouldReturn400()
    {
        var response = await _client.GetAsync("/api/convert?value=1&to=meter");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_MissingToParam_ShouldReturn400()
    {
        var response = await _client.GetAsync("/api/convert?value=1&from=meter");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Convert_ZeroValue_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/convert?value=0&from=meter&to=foot");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        result!.Result.Should().Be(0);
    }

    [Fact]
    public async Task Convert_NegativeValue_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/convert?value=-40&from=celsius&to=fahrenheit");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        // -40°C = -40°F (the crossover point)
        result!.Result.Should().BeApproximately(-40.0, 0.01);
    }

    [Fact]
    public async Task GetUnits_ShouldReturnAllCategories()
    {
        var response = await _client.GetAsync("/api/units");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<List<UnitCategoryInfo>>();
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThanOrEqualTo(5);
        result.Select(c => c.Category).Should().Contain(new[]
        {
            "length", "temperature", "weight", "volume", "speed"
        });
    }

    [Fact]
    public async Task GetUnitsByCategory_ValidCategory_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/units/length");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UnitCategoryInfo>();
        result.Should().NotBeNull();
        result!.Category.Should().Be("length");
        result.Units.Should().Contain("meter");
    }

    [Fact]
    public async Task GetUnitsByCategory_InvalidCategory_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/units/energy");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
