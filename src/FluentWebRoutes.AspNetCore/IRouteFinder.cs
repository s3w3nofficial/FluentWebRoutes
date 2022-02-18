using System.Linq.Expressions;

namespace FluentWebRoutes.AspNetCore;

public interface IRouteFinder
{
    Uri Link<T>(Expression<Action<T>> method) where T : ControllerLink;
    
    Uri Link<T>(Expression<Func<T, Task>> method) where T : ControllerLink;
}