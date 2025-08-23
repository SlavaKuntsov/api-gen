using System.Collections.Generic;
using System.Threading.Tasks;
using ApiGen.Tooling.Models;
using ApiGen.Tooling.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ApiGen.Web.Pages.Routes;

public sealed class IndexModel : PageModel
{
    private readonly IRouteScanner _scanner;
    private readonly IWorkspace _workspace;

    public IndexModel(IRouteScanner scanner, IWorkspace workspace)
    {
        _scanner = scanner;
        _workspace = workspace;
    }

    public IReadOnlyList<ControllerDescriptor> Controllers { get; private set; } = new List<ControllerDescriptor>();

    public async Task OnGetAsync()
    {
        Controllers = await _scanner.ScanAsync(_workspace.Root);
    }
}
