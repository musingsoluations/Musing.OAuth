using System.Linq;
using Musing.OAuth.API.Controllers;
using Xunit;

namespace Musing.OAuth.UnitTests;

public class WeatherForecastControllerTests
{
    [Fact]
    public void Get_ReturnsFiveForecasts_WithSummaries()
    {
        var controller = new WeatherForecastController();
        var result = controller.Get();
        Assert.NotNull(result);
        Assert.Equal(5, result.Count());
        Assert.All(result, f => Assert.False(string.IsNullOrWhiteSpace(f.Summary)));
    }
}
