using ApiGen.Tooling.Models;

namespace ApiGen.Tooling.Services;

public interface ITemplateRenderer
{
    string RenderController(string @namespace, string prefix, string controllerName, IReadOnlyList<MethodTemplate> methods);
    string RenderActionMethod(MethodTemplate method);
}
