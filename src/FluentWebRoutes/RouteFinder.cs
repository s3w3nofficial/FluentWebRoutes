using System.Linq.Expressions;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace FluentWebRoutes;

public class RouteFinder : IRouteFinder
{
    private readonly IOptions<FluentWebRoutesSettings> _options;
    public RouteFinder(IOptions<FluentWebRoutesSettings> options)
    {
        this._options = options;
    }
    
    public Uri Link<T>(Expression<Action<T>> method)
        where T : ControllerLink
    {
        var invocatitonInfo = Helpers.GetInvocation(method);
        return GenerateLink<T>(invocatitonInfo);
    }

    public Uri Link<T>(Expression<Func<T, Task>> method)
        where T : ControllerLink
    {
        var invocatitonInfo = Helpers.GetInvocation(method);
        return GenerateLink<T>(invocatitonInfo);
    }
    
    private Uri GenerateLink<T>(Invocation invocatitonInfo)
    {
        var queryString = 
            $"?{HttpUtility.UrlDecode(string.Join("&", invocatitonInfo.ParameterValues.Select(kvp => $"{kvp.Key}={kvp.Value}")))}";

        var config = this._options.Value.Routes[invocatitonInfo.ProjectName];
        var path = invocatitonInfo.MethodTemplate is not null ? "/" + invocatitonInfo.MethodTemplate : "";
        
        var url = UriHelper.BuildAbsolute(config.Scheme,
            new HostString(config.Host),
            new PathString("/" + invocatitonInfo.ControllerName),
            new PathString(path),
            new QueryString(invocatitonInfo.ParameterValues.Count > 0 ? queryString : ""));

        return new Uri(url);
    }
}