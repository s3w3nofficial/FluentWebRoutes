using System.Linq.Expressions;

namespace FluentWebRoutes;

public interface IRouteFinder
{
    Uri Link<T>(HttpContext context, Expression<Action<T>> method);
}