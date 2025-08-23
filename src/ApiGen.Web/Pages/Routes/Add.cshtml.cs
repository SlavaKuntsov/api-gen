using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiGen.Web.Pages.Routes;

public sealed class AddModel : PageModel
{
    [BindProperty] public string ControllerName { get; set; } = string.Empty;
    [BindProperty] public string Prefix { get; set; } = string.Empty;
    [BindProperty] public string Http { get; set; } = "GET";
    [BindProperty] public string RouteTemplate { get; set; } = string.Empty;
    [BindProperty] public string MethodName { get; set; } = string.Empty;
    [BindProperty] public string ParameterType { get; set; } = string.Empty;
    [BindProperty] public string ParameterName { get; set; } = string.Empty;
    [BindProperty] public string ReturnType { get; set; } = "IActionResult";
    [BindProperty] public string TargetFileRelative { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("Preview", new
        {
            ControllerName,
            Prefix,
            Http,
            RouteTemplate,
            MethodName,
            ParameterType,
            ParameterName,
            ReturnType,
            TargetFileRelative
        });
    }
}
