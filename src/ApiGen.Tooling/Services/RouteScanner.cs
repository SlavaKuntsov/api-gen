using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiGen.Tooling.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiGen.Tooling.Services;

public sealed class RouteScanner : IRouteScanner
{
    public async Task<IReadOnlyList<ControllerDescriptor>> ScanAsync(string root)
    {
        var result = new List<ControllerDescriptor>();
        var files = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var text = await File.ReadAllTextAsync(file);
            var tree = CSharpSyntaxTree.ParseText(text);
            var compilation = tree.GetCompilationUnitRoot();
            var classes = compilation.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var cls in classes)
            {
                if (!IsController(cls)) continue;
                var prefix = GetRoutePrefix(cls);
                var actions = GetActions(cls);
                result.Add(new ControllerDescriptor(cls.Identifier.Text, prefix, actions));
            }
        }
        return result;
    }

    private static bool IsController(ClassDeclarationSyntax cls)
    {
        var hasAttr = cls.AttributeLists
            .SelectMany(a => a.Attributes)
            .Any(a => a.Name.ToString().Contains("ApiController"));
        if (hasAttr) return true;
        var inherits = cls.BaseList?.Types.Any(t => t.Type.ToString().EndsWith("ControllerBase")) ?? false;
        return inherits;
    }

    private static string GetRoutePrefix(ClassDeclarationSyntax cls)
    {
        var attr = cls.AttributeLists.SelectMany(a => a.Attributes)
            .FirstOrDefault(a => a.Name.ToString().Contains("Route"));
        if (attr?.ArgumentList?.Arguments.Count > 0)
        {
            var expr = attr.ArgumentList.Arguments[0].Expression;
            return expr.ToString().Trim('\"');
        }
        return string.Empty;
    }

    private static IReadOnlyList<ActionDescriptor> GetActions(ClassDeclarationSyntax cls)
    {
        var list = new List<ActionDescriptor>();
        foreach (var method in cls.Members.OfType<MethodDeclarationSyntax>())
        {
            var attr = method.AttributeLists.SelectMany(a => a.Attributes)
                .FirstOrDefault(a => a.Name.ToString().StartsWith("Http"));
            if (attr == null) continue;
            var http = attr.Name.ToString()[4..].ToUpperInvariant();
            var template = string.Empty;
            if (attr.ArgumentList?.Arguments.Count > 0)
            {
                template = attr.ArgumentList.Arguments[0].Expression.ToString().Trim('\"');
            }
            var parameters = method.ParameterList.Parameters
                .Select(p => new ParameterDescriptor(p.Type?.ToString() ?? string.Empty, p.Identifier.Text))
                .ToList();
            list.Add(new ActionDescriptor(http, template, method.Identifier.Text, parameters));
        }
        return list;
    }
}
