# FluentWebRoutes

---

## Registration

```csharp
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IRouteFinder, RouteFinder>();
```

## Usage

```csharp
var linkToTest = this._routeFinder.Link<WeatherForecastController>(x => x.Navigation());
var linkToTest = this._routeFinder.Link<WeatherForecastController>(x => x.Test(10));
var linkToTestWithBody = this._routeFinder.Link<WeatherForecastController>(x => x.TestWithBody(10, new WeatherForecast()));
```
