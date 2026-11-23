
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using WeatherMCP.Models;

namespace WeatherMCP.Tools;

[McpServerToolType]
public class WeatherTools
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public WeatherTools(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [McpServerTool]
    [Description("Gets the current weather information for a given city.")]
    public async Task<string> GetCurrentWeather(
        [Description("The name of the city, for example Hyderabad or London.")]
        string city)
    {
        var apiKey = _configuration["WeatherApi:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return "Weather API key is not configured.";
        }

        var client = _httpClientFactory.CreateClient("WeatherApi");

        var url = $"current.json?key={Uri.EscapeDataString(apiKey)}&q={Uri.EscapeDataString(city)}";

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var weatherData = await response.Content.ReadAsStringAsync();

        var weather = JsonSerializer.Deserialize<WeatherResponse>(weatherData,new JsonSerializerOptions
                     {
                        PropertyNameCaseInsensitive = true
                      });

        if (weather?.Location == null ||weather.Current?.Condition == null)
        {
            return "Unable to retrieve weather information.";
        }

        return $"""
                Weather in {weather.Location.Name}, {weather.Location.Country}
                Temperature: {weather.Current.Temp_C} °C
                Condition: {weather.Current.Condition.Text}
                Humidity: {weather.Current.Humidity}%
                Wind Speed: {weather.Current.Wind_Kph} km/h
                """;
    }
}
