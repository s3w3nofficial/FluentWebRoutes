using FluentWebRoutes.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using FluentWebRoutes.SourceGenerator.ControllerLinks;
 // Auto-generated code
using FluentWebRoutes; 
using FluentWebRoutes.Attributes; 

namespace FluentWebRoutes.SourceGenerator.ControllerLinks
{
    [ProjectName("FluentWebRoutes.SourceGenerator.Sample")]
    public class HomePageControllerLink : ControllerLink 
    {

         [HttpGet("navigation", Name = nameof(Navigation))]
         internal void Navigation()
         {
 
         }
 
         [HttpGet("get/{id}", Name = nameof(Get))]
         internal void Get( id)
         {
 
         }
 
         [HttpPut("put/{id}", Name = nameof(PutWeather))]
         internal void PutWeather( id,  weatherForecast)
         {
 
         }
 
     }
 }
 