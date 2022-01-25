using System.Linq.Expressions;

namespace FluentWebRoutes;

public interface IRouteFinder
{
    Uri Link<T>(Expression<Action<T>> method);
}