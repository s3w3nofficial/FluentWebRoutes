using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;

namespace FluentWebRoutes.AspNetCore.Tests;

[Route("[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet("test-endpoint", Name = nameof(Test))]
    public IActionResult Test()
    {
        return Ok();
    }

    [HttpPost("test-post-endpoint", Name = nameof(TestPost))]
    public IActionResult TestPost()
    {
        return Ok();
    }
}

public class TestLinkGenerator : LinkGenerator
{
    public override string? GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values,
        RouteValueDictionary? ambientValues = null, PathString? pathBase = null,
        FragmentString fragment = new FragmentString(), LinkOptions? options = null)
    {
        throw new System.NotImplementedException();
    }

    public override string? GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values,
        PathString pathBase = new PathString(), FragmentString fragment = new FragmentString(),
        LinkOptions? options = null)
    {
        throw new System.NotImplementedException();
    }

    public override string? GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values,
        RouteValueDictionary? ambientValues = null, string? scheme = null, HostString? host = null,
        PathString? pathBase = null, FragmentString fragment = new FragmentString(), LinkOptions? options = null)
    {
        return $"{values["action"]}:{values["controller"]}";
    }

    public override string? GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, string? scheme, HostString host,
        PathString pathBase = new PathString(), FragmentString fragment = new FragmentString(),
        LinkOptions? options = null)
    {
        throw new System.NotImplementedException();
    }
}

public class RouteFinderTests 
{
    [Test]
    public void TestLinkWithoutValue()
    {
        // Arrange
        var httpContextAccessor = new HttpContextAccessor();
        var httpContext = new DefaultHttpContext();
        httpContextAccessor.HttpContext = httpContext;

        var linkGenerator = new TestLinkGenerator();
        var routeFinder = new RouteFinder(linkGenerator, httpContextAccessor);
        
        // Act
        var testRoute = routeFinder.Link<SampleController>(x => x.Test());
        
        // Assert
        Assert.AreEqual( "test:Sample", testRoute!.AbsoluteUri);
    }

    [Test]
    public void TestGetInvocation()
    {
       // Arrange
       
       // Act
       var invocation = Helpers.GetInvocation<SampleController>(x => x.Test());
       var invocation2 = Helpers.GetInvocation<SampleController>(x => x.TestPost());

       // Assert
       Assert.AreEqual("GET", invocation.MethodType);
       Assert.AreEqual("POST", invocation2.MethodType);
    }
}