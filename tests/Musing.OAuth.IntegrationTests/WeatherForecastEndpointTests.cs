using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Musing.OAuth.IntegrationTests;

public class WeatherForecastEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherForecastEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async System.Threading.Tasks.Task GetWeatherForecast_ReturnsFiveItems()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/weatherforecast");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var forecasts = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts!.Count());
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string? Summary { get; set; }
}
