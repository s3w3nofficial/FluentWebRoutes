using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;

namespace FluentWebRoutes;

public static class Helpers
{
    public static Invocation GetInvocation<T>(Expression<Func<T, Task>> action)
        where T : ControllerBase
    {
        if (action.Body is not MethodCallExpression callExpression)
            throw new ArgumentException("Action must be a method call", nameof(action));

        return GetInvocationFromMethodCall(callExpression);
    }
    
    public static Invocation GetInvocation<T>(Expression<Action<T>> action)
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

        var methodType = callExpression.Method.GetCustomAttributes<HttpMethodAttribute>()
            .FirstOrDefault()?.HttpMethods.FirstOrDefault() ?? "GET";

        return new Invocation
        {
            ParameterValues = result,
            MethodName = callExpression.Method.Name,
            MethodType = methodType
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