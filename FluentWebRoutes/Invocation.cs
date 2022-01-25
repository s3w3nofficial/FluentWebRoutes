namespace FluentWebRoutes;

internal record Invocation
{
    public string? MethodName { get; init; }

    public IDictionary<string, object>? ParameterValues { get; init; }
}