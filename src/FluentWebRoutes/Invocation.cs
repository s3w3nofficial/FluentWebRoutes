namespace FluentWebRoutes;

public record Invocation
{
    public string? MethodName { get; init; }
    public string? MethodType { get; init; }
    public IDictionary<string, object>? ParameterValues { get; init; }
}