namespace FluentWebRoutes;

public record Invocation
{
    public string? ProjectName { get; set; }
    public string? ControllerName { get; set; }
    public string? MethodName { get; init; }
    public string? MethodTemplate { get; set; }
    public string? MethodType { get; init; }
    public IDictionary<string, string>? RouteParameterValues { get; init; }
    public IDictionary<string, object>? ParameterValues { get; init; }
}