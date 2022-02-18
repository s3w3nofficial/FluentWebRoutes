using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FluentWebRoutes.SourceGenerator
{
    [Generator]
    public class ControllerLinkSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxTrees = context.Compilation.SyntaxTrees;

            foreach (var syntaxTree in syntaxTrees)
            {
                var controllers = syntaxTree
                    .GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>()
                    .Where(x => x.AttributeLists.Any(a => a.ToString() == "[ApiController]"));

                foreach (var controller in controllers)
                {
                    var usingDirectives = syntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
                    var usingDirectivesAsText = string.Join("\r\n", usingDirectives);
                    var sourceBuilder = new StringBuilder(usingDirectivesAsText);
                    sourceBuilder.AppendLine();

                    var className = controller.Identifier.ToString().Replace("Controller", "ControllerLink");

                    var actions = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();

                    var classSource = $@" // Auto-generated code
using FluentWebRoutes; 
using FluentWebRoutes.Attributes; 

namespace FluentWebRoutes.SourceGenerator.ControllerLinks
{{
    [ProjectName(""{context.Compilation.AssemblyName}"")]
    public class {className} : ControllerLink 
    {{
";
                    var classSourceBuilder = new StringBuilder(classSource);

                    foreach (var action in actions)
                    {
                        var attributes = action.AttributeLists;
                        var actionName = action.Identifier.ToString();

                        // exclude parameters
                        var actionParameters = action.ParameterList.Parameters
                            .Where(ap =>
                            {
                                if (ap.Type is null)
                                    return false;

                                /*
                                var model = Compilation.GetSemanticModel(controller.SyntaxTree);
                                
                                if (model.GetTypeInfo(ap.Type).Type.IsValueType)
                                    return true;

                                return false;
                                return ap.AttributeLists.Any();
                                */
                                return true;
                            })
                            .Where(ap => ap.Type?.ToString() != nameof(CancellationToken));

                        var parameters = string.Join(", ",
                            actionParameters.Select(p => $"{p.Type} {p.Identifier.ToString()}"));

                        string source = $@"
         {attributes.FirstOrDefault().ToString()}
         internal void {actionName}({parameters})
         {{
 
         }}
 ";
                        classSourceBuilder.Append(source);
                    }

                    var classSourceEnd = $@"
     }}
 }}
 ";
                    classSourceBuilder.Append(classSourceEnd);
                    context.AddSource($"{className}.g.cs", sourceBuilder.Append(classSourceBuilder).ToString());
                }
            }
        }
    }
}