using Microsoft.AspNetCore.Mvc;

namespace FluentWebRoutes.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : BaseController 
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
        var linkToSelf = this._routeFinder.Link<WeatherForecastController>(
            x => x.Navigation());
        
        var link = this._routeFinder.Link<WeatherForecastController>( 
            x => x.Get());

        var linkToTest = this._routeFinder.Link<WeatherForecastController>(
            x => x.Test(420));
        
        var linkToAsyncTest = this._routeFinder.Link<WeatherForecastController>(
            x => x.AsyncTest(69));

        var linkToTestWithBody = this._routeFinder.Link<WeatherForecastController>(
            x => x.TestWithBody(10, new WeatherForecast()));
        
        var linkToAsyncTestWithBody = this._routeFinder.Link<WeatherForecastController>(
            x => x.AsyncTestWithBody(10, new WeatherForecast()));

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
            },
            AsyncTest = new
            {
                Rel = new [] { "item" },
                Href = linkToAsyncTest 
            },
            TestWithBody = new
            {
                Rel = new [] { "item" },
                Href = linkToTestWithBody 
            },
            AsyncTestWithBody = new
            {
                Rel = new [] { "item" },
                Href = linkToAsyncTestWithBody
            }
        });
    }

    [HttpGet("test/{id:int}", Name = nameof(Test))]
    public IActionResult Test(int id)
    {
        return Ok(new
        {
            Id = id
        });
    }

    [HttpGet("asyncTest/{id:int}", Name = nameof(AsyncTest))]
    public async Task<IActionResult> AsyncTest(int id)
    {
        var task = Task.FromResult(new
        {
            Id = id
        });

        return Ok(await task);
    }

    [HttpPut("testWithBody/{id:int}", Name = nameof(TestWithBody))]
    public IActionResult TestWithBody(int id, [FromBody] WeatherForecast weatherForecast)
    {
        return NoContent();
    }
    
    [HttpPut("asyncTestWithBody/{id:int}", Name = nameof(AsyncTestWithBody))]
    public async Task<IActionResult> AsyncTestWithBody(int id, [FromBody] WeatherForecast weatherForecast)
    {
        await Task.CompletedTask;
        return NoContent();
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