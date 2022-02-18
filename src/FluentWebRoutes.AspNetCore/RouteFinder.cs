using System.Linq.Expressions;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;

namespace FluentWebRoutes.AspNetCore;

public class RouteFinder : IRouteFinder
{
    private readonly IOptions<FluentWebRoutesSettings> _options;
    private readonly TemplateBinderFactory _templateBinderFactory;
    public RouteFinder(IOptions<FluentWebRoutesSettings> options, TemplateBinderFactory templateBinderFactory)
    {
        this._options = options;
        this._templateBinderFactory = templateBinderFactory;
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

        invocatitonInfo.MethodTemplate ??= "";
        var routePattern = RoutePatternFactory.Parse(invocatitonInfo.MethodTemplate);
        var binder = this._templateBinderFactory.Create(routePattern);
        var path = binder.BindValues(new RouteValueDictionary(invocatitonInfo.RouteParameterValues));

        var url = UriHelper.BuildAbsolute(config.Scheme,
            new HostString(config.Host),
            new PathString("/" + invocatitonInfo.ControllerName),
            new PathString("/" + path),
            new QueryString(invocatitonInfo.ParameterValues.Count > 0 ? queryString : ""));

        return new Uri(url);
    }
}