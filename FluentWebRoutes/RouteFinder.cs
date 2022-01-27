using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FluentWebRoutes;

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
        var invocatitonInfo = GetInvocation(method);
        return GenerateLink<T>(invocatitonInfo);
    }

    public Uri Link<T>(Expression<Func<T, Task>> method)
        where T : ControllerBase
    {
        var invocatitonInfo = GetInvocation(method);
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
    
    private static Invocation GetInvocation<T>(Expression<Func<T, Task>> action)
        where T : ControllerBase
    {
        if (action.Body is not MethodCallExpression callExpression)
            throw new ArgumentException("Action must be a method call", nameof(action));

        return GetInvocationFromMethodCall(callExpression);
    }
    
    private static Invocation GetInvocation<T>(Expression<Action<T>> action)
        where T : ControllerBase
    {
        if (action.Body is not MethodCallExpression callExpression)
            throw new ArgumentException("Action must be a method call", nameof(action));

        return GetInvocationFromMethodCall(callExpression);
    }

    private static Invocation GetInvocationFromMethodCall(MethodCallExpression callExpression)
    {
        var values = callExpression.Arguments.Select(ReduceToConstant).ToList();
        var names = callExpression
            .Method
            .GetParameters()
            .Where(p => p.GetCustomAttributes(false).Any(a => a 
                is FromBodyAttribute 
                or FromFormAttribute 
                or FromServicesAttribute) == false)
            .Where(p => p.ParameterType != typeof(CancellationToken))
            .Select(i => i.Name)
            .ToList();

        var result = new Dictionary<string, object>();
        for (var i = 0; i < names.Count; i++)
        {
            if (values[i] is ValueType)
                result.Add(names[i]!, values[i]);
            else
            {
                // TODO find a better way to create dictionary from object
                var json = JsonConvert.SerializeObject(values[i]);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                foreach (var (key, value) in dictionary)
                    result.Add(key.ToLower(), value);
            }
        }
        
        return new Invocation
        {
            ParameterValues = result,
            MethodName = callExpression.Method.Name
        };
    }
    
    private static object ReduceToConstant(Expression expression)
    {
        var objectMember = Expression.Convert(expression, typeof(object));
        var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        var getter = getterLambda.Compile();
        return getter();
    }
}