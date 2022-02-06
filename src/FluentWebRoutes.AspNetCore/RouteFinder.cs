using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FluentWebRoutes.AspNetCore;

public class RouteFinder : IRouteFinder
{
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _context;
    public RouteFinder(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        this._linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        this._context = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }
    
    public Uri Link<T>(Expression<Action<T>> method)
        where T : ControllerBase
    {
        var invocatitonInfo = Helpers.GetInvocation(method);
        return GenerateLink<T>(invocatitonInfo);
    }

    public Uri Link<T>(Expression<Func<T, Task>> method)
        where T : ControllerBase
    {
        var invocatitonInfo = Helpers.GetInvocation(method);
        return GenerateLink<T>(invocatitonInfo);
    }
    private Uri GenerateLink<T>(Invocation invocatitonInfo)
        where T : ControllerBase
    {
        var controllerName = typeof(T)
            .ToGenericTypeString()
            .Replace("Controller", "");
        
        var url = this._linkGenerator
            .GetUriByAction(this._context, 
                action: invocatitonInfo.MethodName,
                controller: controllerName,
                values: invocatitonInfo.ParameterValues);

        if (url is null)
            throw new Exception("url generation failed");
        
        return new Uri(url);
    }
}