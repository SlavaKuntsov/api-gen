using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGen.Tooling.Models;
using ApiGen.Tooling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiGen.Web.Pages.Routes;

public sealed class PreviewModel : PageModel
{
    private readonly ITemplateRenderer _renderer;
    private readonly IDiffService _diff;
    private readonly IWorkspace _workspace;

    public PreviewModel(ITemplateRenderer renderer, IDiffService diff, IWorkspace workspace)
    {
        _renderer = renderer;
        _diff = diff;
        _workspace = workspace;
    }

    [BindProperty(SupportsGet = true)] public string ControllerName { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string Prefix { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string Http { get; set; } = "GET";
    [BindProperty(SupportsGet = true)] public string RouteTemplate { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string MethodName { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string ParameterType { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string ParameterName { get; set; } = string.Empty;
    [BindProperty(SupportsGet = true)] public string ReturnType { get; set; } = "IActionResult";
    [BindProperty(SupportsGet = true)] public string TargetFileRelative { get; set; } = string.Empty;

    public string Diff { get; private set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var method = BuildMethod();
        var content = _renderer.RenderController("Api", Prefix, ControllerName, new[] { method });
        var oldText = await _workspace.ReadFileAsync(TargetFileRelative);
        Diff = _diff.Diff(oldText, content);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var method = BuildMethod();
        var content = _renderer.RenderController("Api", Prefix, ControllerName, new[] { method });
        await _workspace.WriteFileAsync(TargetFileRelative, content);
        return RedirectToPage("Index");
    }

    private MethodTemplate BuildMethod()
    {
        var parameters = new List<ParameterDescriptor>();
        if (!string.IsNullOrWhiteSpace(ParameterType) && !string.IsNullOrWhiteSpace(ParameterName))
        {
            parameters.Add(new ParameterDescriptor(ParameterType, ParameterName));
        }
        return new MethodTemplate(Http, RouteTemplate, MethodName, ReturnType, parameters);
    }
}
