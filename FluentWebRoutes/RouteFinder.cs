using System.Linq.Expressions;

namespace FluentWebRoutes;

public class RouteFinder : IRouteFinder
{
    private readonly LinkGenerator _linkGenerator;
    private readonly HttpContext _context;
    public RouteFinder(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        this._linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        this._context = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext));
    }
    
    public Uri Link<T>(Expression<Action<T>> method)
    {
        var invocatitonInfo = GetInvocation(method);
        var controllerName = typeof(T).ToGenericTypeString().Replace("Controller", "");
        
        var url = this._linkGenerator
            .GetUriByAction(this._context, 
                action: invocatitonInfo.MethodName,
                controller: controllerName,
                values: invocatitonInfo.ParameterValues);

        if (url is null)
            throw new Exception("url generation failed");
        
        return new Uri(url);
    }
    
    private Invocation GetInvocation<T>(Expression<Action<T>> action)
    {
        if (action.Body is not MethodCallExpression callExpression)
            throw new ArgumentException("Action must be a method call", nameof(action));

        var values = callExpression.Arguments.Select(ReduceToConstant).ToArray();
        var names = callExpression
            .Method
            .GetParameters()
            .Select(i => i.Name)
            .ToList();

        var result = new Dictionary<string, object>();
        foreach (var name in names)
        {
            foreach (var value in values)
            {
                if (name is not null)
                    result.Add(name, value);
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