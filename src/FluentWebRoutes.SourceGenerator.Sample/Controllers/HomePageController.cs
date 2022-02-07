using Microsoft.AspNetCore.Mvc;
using FluentWebRoutes.SourceGenerator.ControllerLinks;

namespace FluentWebRoutes.SourceGenerator.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class HomePageController : ControllerBase
{
    private readonly IRouteFinder _routeFinder;
    public HomePageController(IRouteFinder routeFinder)
    {
        this._routeFinder = routeFinder;
    }

    [HttpGet("navigation", Name = nameof(Navigation))]
    public IActionResult Navigation()
    {
        return Ok(new
        {
            Self = this._routeFinder.Link<HomePageControllerLink>(c => c.Navigation()),
            Get = this._routeFinder.Link<HomePageControllerLink>(c => c.Get(10)),
            Put = this._routeFinder.Link<HomePageControllerLink>(c => c.PutWeather(10, new WeatherForecast
            {
                Date = DateTime.Now,
                Summary = "wwwws",
                TemperatureC = 30
            })),
            WeatherForecast = this._routeFinder.Link<WeatherForecastControllerLink>(c => c.Get())
        });
    }

    [HttpGet("get/{id}", Name = nameof(Get))]
    public IActionResult Get(int id)
    {
        return Ok(id);
    }
    
    [HttpPut("put/{id}", Name = nameof(PutWeather))]
    public IActionResult PutWeather(int id, FluentWebRoutes.SourceGenerator.Sample.WeatherForecast weatherForecast)
    {
        return Ok(id);
    }
}