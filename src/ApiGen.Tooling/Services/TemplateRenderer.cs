using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApiGen.Tooling.Models;
using Scriban;

namespace ApiGen.Tooling.Services;

public sealed class TemplateRenderer : ITemplateRenderer
{
    private readonly string _root;

    public TemplateRenderer(string root)
    {
        _root = root;
    }

    public string RenderController(string @namespace, string prefix, string controllerName, IReadOnlyList<MethodTemplate> methods)
    {
        var tplPath = Path.Combine(_root, "controller.sbn");
        var template = Template.Parse(File.ReadAllText(tplPath));
        var methodsModel = methods.Select(m => new
        {
            http = m.Http,
            template = m.Template,
            name = m.Name,
            return_type = m.ReturnType,
            parameters = string.Join(", ", m.Parameters.Select(p => $"{p.Type} {p.Name}")),
            return_stmt = GetReturnStatement(m.ReturnType)
        }).ToList();
        var model = new { @namespace, prefix, controller_name = controllerName, methods = methodsModel };
        return template.Render(model);
    }

    public string RenderActionMethod(MethodTemplate method)
    {
        var tplPath = Path.Combine(_root, "action_method.sbn");
        var template = Template.Parse(File.ReadAllText(tplPath));
        var model = new
        {
            http = method.Http,
            route_template = method.Template,
            method_name = method.Name,
            return_type = method.ReturnType,
            parameters = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}")),
            return_stmt = GetReturnStatement(method.ReturnType)
        };
        return template.Render(model);
    }

    private static string GetReturnStatement(string returnType) => returnType switch
    {
        "IActionResult" => "Ok()",
        "Task<IActionResult>" => "Task.FromResult<IActionResult>(Ok())",
        _ => "default!"
    };
}
