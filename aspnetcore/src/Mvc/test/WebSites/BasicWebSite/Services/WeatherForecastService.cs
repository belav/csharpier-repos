using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BasicWebSite.Services;

public class WeatherForecastService
{
    private readonly HttpClient _httpClient;

    public WeatherForecastService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
    {
        var result = await _httpClient.GetAsync("/WeatherData");
        result.EnsureSuccessStatusCode();
        var dataString = await result.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<WeatherForecast[]>(
            dataString,
            new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }
        );
        return weatherData;
    }
}

public class WeatherForecast
{
    public string DateFormatted { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }
    public int TemperatureF { get; set; }
}
