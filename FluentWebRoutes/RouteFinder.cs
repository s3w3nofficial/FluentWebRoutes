using System.Linq.Expressions;

namespace FluentWebRoutes;

public class RouteFinder : IRouteFinder
{
    private LinkGenerator _linkGenerator;
    public RouteFinder(LinkGenerator linkGenerator)
    {
        this._linkGenerator = linkGenerator;
    }
    
    public Uri Link<T>(HttpContext context, Expression<Action<T>> method)
    {
        var invocatitonInfo = GetInvocation(method);
        var controllerName = typeof(T).ToGenericTypeString().Replace("Controller", "");
        
        var url = this._linkGenerator
            .GetUriByAction(context, 
                action: invocatitonInfo.MethodName,
                controller: controllerName,
                values: invocatitonInfo.ParameterValues);
        
        return new Uri(url ?? "");
    }
    
    private Invocation GetInvocation<T>(Expression<Action<T>> action)
    {
        if (!(action.Body is MethodCallExpression))
        {
            throw new ArgumentException("Action must be a method call", "action");
        }

        var callExpression = (MethodCallExpression)action.Body;

        var values = callExpression.Arguments.Select(ReduceToConstant).ToArray();
        var names = callExpression
            .Method
            .GetParameters()
            .Select(i => i.Name)
            .ToList();

        IDictionary<string, object> result = new Dictionary<string, object>();
        foreach (var name in names)
        foreach (var value in values)
            result.Add(name, value);

        return new Invocation
        {
            ParameterValues = result,
            MethodName = callExpression.Method.Name
        };
    }
    
    private object ReduceToConstant(Expression expression)
    {
        var objectMember = Expression.Convert(expression, typeof(object));
        var getterLambda = Expression.Lambda<Func<object>>(objectMember);
        var getter = getterLambda.Compile();
        return getter();
    }
    
    private class Invocation
    {
        public string MethodName { get; set; }

        public IDictionary<string, object> ParameterValues { get; set; }
    }
}