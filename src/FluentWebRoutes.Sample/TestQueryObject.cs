namespace FluentWebRoutes.Sample;

public record TestQueryObject
{
    public int Offset { get; init; }
    public int Limit { get; init; }
}