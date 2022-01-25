using Microsoft.AspNetCore.Mvc;

namespace FluentWebRoutes.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IRouteFinder _routeFinder;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IRouteFinder routeFinder)
    {
        _logger = logger;
        this._routeFinder = routeFinder;
    }

    [HttpGet("navigation", Name = nameof(Navigation))]
    public IActionResult Navigation()
    {
        var linkToSelf = this._routeFinder.Link<WeatherForecastController>(HttpContext,
            x => x.Navigation());
        
        var link = this._routeFinder.Link<WeatherForecastController>(HttpContext, 
            x => x.Get());

        var linkToTest = this._routeFinder.Link<WeatherForecastController>(HttpContext,
            x => x.Test(10));

        return Ok(new
        {
            Self = new
            {
              Rel = new [] { "self" },
              Href = linkToSelf 
            },
            GetWeatherForecast = new
            {
                Rel = new [] { "collection" },
                Href = link
            },
            Test = new
            {
                Rel = new [] { "item" },
                Href = linkToTest
            }
        });
    }

    [HttpGet("test/{id:int}")]
    public IActionResult Test(int id)
    {
        return Ok(new
        {
            Id = id
        });
    }

    [HttpGet("weatherForecast", Name = nameof(Get))]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}