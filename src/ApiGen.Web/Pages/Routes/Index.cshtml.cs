using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGen.Tooling.Models;
using ApiGen.Tooling.Services;
using ApiGen.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiGen.Web.Pages.Routes;

public sealed class IndexModel : PageModel
{
    private readonly IRouteScanner _scanner;
    private readonly ITemplateRenderer _renderer;
    private readonly IDiffService _diff;
    private readonly IWorkspace _workspace;

    public IndexModel(IRouteScanner scanner, ITemplateRenderer renderer, IDiffService diff, IWorkspace workspace)
    {
        _scanner = scanner;
        _renderer = renderer;
        _diff = diff;
        _workspace = workspace;
    }

    public IReadOnlyList<ControllerDescriptor> Controllers { get; private set; } = new List<ControllerDescriptor>();

    [BindProperty] public string ControllerName { get; set; } = string.Empty;
    [BindProperty] public string Prefix { get; set; } = string.Empty;
    [BindProperty] public string Http { get; set; } = "GET";
    [BindProperty] public string RouteTemplate { get; set; } = string.Empty;
    [BindProperty] public string MethodName { get; set; } = string.Empty;
    [BindProperty] public List<ParameterModel> Parameters { get; set; } = new();
    [BindProperty] public string ReturnType { get; set; } = "IActionResult";
    [BindProperty] public string TargetFileRelative { get; set; } = string.Empty;

    public string? Diff { get; private set; }

    public async Task OnGetAsync()
    {
        Controllers = await _scanner.ScanAsync(_workspace.Root);
    }

    public async Task<IActionResult> OnPostPreviewAsync()
    {
        await LoadControllersAsync();
        EnsureTargetFile();
        var method = BuildMethod();
        var content = _renderer.RenderController("Api", Prefix, ControllerName, new[] { method });
        var oldText = await _workspace.ReadFileAsync(TargetFileRelative);
        Diff = _diff.Diff(oldText, content);
        return Page();
    }

    public async Task<IActionResult> OnPostApplyAsync()
    {
        EnsureTargetFile();
        var method = BuildMethod();
        var content = _renderer.RenderController("Api", Prefix, ControllerName, new[] { method });
        await _workspace.WriteFileAsync(TargetFileRelative, content);
        return RedirectToPage();
    }

    private async Task LoadControllersAsync()
    {
        Controllers = await _scanner.ScanAsync(_workspace.Root);
    }

    private MethodTemplate BuildMethod()
    {
        var descriptors = new List<ParameterDescriptor>();
        foreach (var p in Parameters)
        {
            if (!string.IsNullOrWhiteSpace(p.Type) && !string.IsNullOrWhiteSpace(p.Name))
                descriptors.Add(new ParameterDescriptor(p.Type, p.Name));
        }
        return new MethodTemplate(Http, RouteTemplate, MethodName, ReturnType, descriptors);
    }

    private void EnsureTargetFile()
    {
        if (string.IsNullOrWhiteSpace(TargetFileRelative) && !string.IsNullOrWhiteSpace(ControllerName))
        {
            TargetFileRelative = $"Api/Generated/{ControllerName}.cs";
        }
    }
}
