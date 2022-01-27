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
        
        var linkToTest = this._routeFinder.Link<WeatherForecastController>(
            x => x.Test(420));
        
        var linkToAsyncTest = this._routeFinder.Link<WeatherForecastController>(
            x => x.AsyncTest(69));

        var linkToTestWithBody =
            this._routeFinder.Link<WeatherForecastController>(x => x.TestWithBody(42, new TestBodyObject()));
        
        var linkToAsyncTestWithBody = this._routeFinder.Link<WeatherForecastController>(
            x => x.AsyncTestWithBody(666, new TestBodyObject()));

        var linkToTestWithQueryObject = this._routeFinder.Link<WeatherForecastController>(x =>
            x.TestWithQueryObject(new TestQueryObject
            {
                Offset = 69,
                Limit = 420
            }));

        var linkToTestWithCancellationToken =
            this._routeFinder.Link<WeatherForecastController>(x =>
                x.TestWithCancellationToken(new CancellationToken()));

        return Ok(new
        {
            Self = new
            {
              Rel = new [] { "self" },
              Href = linkToSelf 
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
            },
            TestWithQueryObject = new
            {
                Rel = new [] { "item" },
                Href = linkToTestWithQueryObject 
            },
            TestWithCancellationToken = new
            {
                Rel = new [] { "item" },
                Href = linkToTestWithCancellationToken
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
    public IActionResult TestWithBody(int id, [FromBody] TestBodyObject testBodyObject)
    {
        return NoContent();
    }
    
    [HttpPut("asyncTestWithBody/{id:int}", Name = nameof(AsyncTestWithBody))]
    public async Task<IActionResult> AsyncTestWithBody(int id, [FromBody] TestBodyObject testBodyObject)
    {
        await Task.CompletedTask;
        return NoContent();
    }

    [HttpGet("testWithQueryObject", Name = nameof(TestWithQueryObject))]
    public IActionResult TestWithQueryObject([FromQuery] TestQueryObject query)
    {
        return Ok(query);
    }

    [HttpGet("testWithCancellationToken", Name = nameof(TestWithCancellationToken))]
    public IActionResult TestWithCancellationToken(CancellationToken cancellationToken)
    {
        return NoContent();
    }
}