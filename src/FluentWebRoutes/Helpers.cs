using System.Linq.Expressions;
using System.Reflection;
using FluentWebRoutes.Attributes;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;

namespace FluentWebRoutes;

public static class Helpers
{
    public static Invocation GetInvocation<T>(Expression<Func<T, Task>> action)
        where T : ControllerLink
    {
        if (action.Body is not MethodCallExpression callExpression)
            throw new ArgumentException("Action must be a method call", nameof(action));

        return GetInvocationFromMethodCall<T>(callExpression);
    }
    
    public static Invocation GetInvocation<T>(Expression<Action<T>> action)
        where T : ControllerLink
    {
        if (action.Body is not MethodCallExpression callExpression)
            throw new ArgumentException("Action must be a method call", nameof(action));

        return GetInvocationFromMethodCall<T>(callExpression);
    }

    private static Invocation GetInvocationFromMethodCall<T>(MethodCallExpression callExpression)
        where T : ControllerLink
    {
        var httpMethodAttribute = callExpression.Method.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();
        var template = httpMethodAttribute?.Template;
        var methodType = httpMethodAttribute?.HttpMethods.FirstOrDefault() ?? "GET";
        
        var values = callExpression.Arguments.Select(ReduceToConstant).ToList();
        var names = callExpression
            .Method
            .GetParameters()
            .Select(i => i.Name)
            .ToList();

        var parameters = names.Zip(values, (k, v) => new {k, v})
            .ToDictionary(x => x.k, x => x.v);

        string filledTemplate = null;
        // filter out route parameters
        if (template is not null)
        {
            var routeParameters = GetRouteParameters(template, parameters);
            parameters = parameters
                .Where(p => !routeParameters.Keys.Contains(p.Key))
                .ToDictionary(x => x.Key, x => x.Value);
            filledTemplate = FillTemplate(template, routeParameters);
        }

        var queryParameters = new Dictionary<string, object>();

        foreach (var (key, value) in parameters)
        {
            if (value is ValueType)
                queryParameters.Add(key, value);
            else
            {
                // TODO find a better way to create dictionary from object
                var json = JsonConvert.SerializeObject(value);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                foreach (var (k, v) in dictionary)
                    queryParameters.Add(k.ToLower(), v);
            }
        }
        
        var projectName = typeof(T).CustomAttributes
            .FirstOrDefault(ca => ca.AttributeType == typeof(ProjectNameAttribute))
            ?.ConstructorArguments.FirstOrDefault().Value as string;
        
        return new Invocation
        {
            ProjectName = projectName,
            ParameterValues = queryParameters,
            ControllerName = typeof(T).Name.Replace("ControllerLink", ""),
            MethodName = callExpression.Method.Name,
            MethodTemplate = filledTemplate, 
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
    
    private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
    {
        var result = new RouteValueDictionary();

        foreach (var parameter in parsedTemplate.Parameters)
        {
            if (parameter.DefaultValue != null)
            {
                result.Add(parameter.Name, parameter.DefaultValue);
            }
        }

        return result;
    }
    
    private static Dictionary<string, object> GetRouteParameters(string routeTemplate, Dictionary<string, object> parameters)
    {
        var template = TemplateParser.Parse(routeTemplate);
        return parameters
            .Where(p => template.Parameters.Any(tp => tp.Name == p.Key))
            .ToDictionary(x => x.Key, x => x.Value);
    }
    
    private static string FillTemplate(string routeTemplate, Dictionary<string, object> routeParameters)
    {
        foreach (var (key, value) in routeParameters)
            routeTemplate = routeTemplate.Replace("{" + key + "}", value.ToString());

        return routeTemplate;
    }
}