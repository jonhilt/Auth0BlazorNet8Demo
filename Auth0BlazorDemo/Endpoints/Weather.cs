using Auth0BlazorDemo.Shared;

namespace Auth0BlazorDemo.Endpoints;

public static class Weather
{
    public static void MapWeatherEndpoints(this WebApplication app)
    {
        app.MapGet("api/weather", async (HttpContext context) =>
        {
            var weatherForecasts = await GetWeather();
            return Results.Ok(weatherForecasts);
        });
    }

    private static async Task<WeatherForecast[]> GetWeather()
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[]
            { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToArray();
    }
}