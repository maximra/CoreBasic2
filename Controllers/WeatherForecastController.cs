using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {

        _logger.LogInformation("Today's forecast requested at {time}", DateTime.Now);
        AppLogger.Instance.Information("Today's forecast requested at {time}", DateTime.Now);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],

            // NEW: Assign random wind speed
            WindSpeedKph = Random.Shared.Next(0, 100)
        })
        .ToArray();
    }

    // NEW endpoint -> /WeatherForecast/today
    [HttpGet("today", Name = "GetTodayForecast")]
    public WeatherForecast GetToday()
    {
        _logger.LogInformation("Today's forecast requested at {time}", DateTime.Now);
        AppLogger.Instance.Information("Today's forecast requested at {time}", DateTime.Now);
        return new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            WindSpeedKph = Random.Shared.Next(0, 100)
        };
    }
    //----NEW FEATURES ----- /////
    private static readonly List<WeatherForecast> _forecasts = new(); // new addition.
    // POST endpoint -> /WeatherForecast
    [HttpPost(Name = "CreateWeatherForecast")]
    public IActionResult Create([FromBody] WeatherForecast newForecast)
    {
        if (newForecast == null)
        {
            _logger.LogWarning("Attempted to create a null forecast at {time}", DateTime.Now);
            AppLogger.Instance.Information("Attempted to create a null forecast at {time}", DateTime.Now);
            return BadRequest("Forecast cannot be null.");
        }
        _forecasts.Add(newForecast); // Stored in memory
        // Log creation
        _logger.LogInformation(
            "New forecast created for {date}: Temp {temp}°C, Summary '{summary}', Wind {wind} Kph at {time}",
            newForecast.Date,
            newForecast.TemperatureC,
            newForecast.Summary,
            newForecast.WindSpeedKph,
            DateTime.Now
        );

        AppLogger.Instance.Information(
            "New forecast created for {date}: Temp {temp}°C, Summary '{summary}', Wind {wind} Kph at {time}",
            newForecast.Date,
            newForecast.TemperatureC,
            newForecast.Summary,
            newForecast.WindSpeedKph,
            DateTime.Now
        );
        return CreatedAtAction(nameof(GetToday), new { }, newForecast);
    }

    [HttpGet("all", Name = "GetAllForecasts")]
    public IEnumerable<WeatherForecast> GetAll()
    {
        _logger.LogInformation("User made a request for all data at:  {time}", DateTime.Now);
        AppLogger.Instance.Information("User made a request for all data at:  {time}", DateTime.Now);
        return _forecasts;
    }


}
