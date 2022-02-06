using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace FluentWebRoutes.AspNetCore;

public interface IRouteFinder
{
    Uri Link<T>(Expression<Action<T>> method) where T : ControllerBase;
    
    Uri Link<T>(Expression<Func<T, Task>> method) where T : ControllerBase;
}