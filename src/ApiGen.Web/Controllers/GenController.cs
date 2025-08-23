using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiGen.Tooling.Models;
using ApiGen.Tooling.Services;
using ApiGen.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGen.Web.Controllers;

[ApiController]
[Route("gen")]
public sealed class GenController : ControllerBase
{
    private readonly IRouteScanner _scanner;
    private readonly ITemplateRenderer _renderer;
    private readonly IDiffService _diff;
    private readonly IWorkspace _workspace;

    public GenController(IRouteScanner scanner, ITemplateRenderer renderer, IDiffService diff, IWorkspace workspace)
    {
        _scanner = scanner;
        _renderer = renderer;
        _diff = diff;
        _workspace = workspace;
    }

    [HttpGet("routes")]
    public async Task<IReadOnlyList<ControllerDescriptor>> GetRoutes()
    {
        return await _scanner.ScanAsync(_workspace.Root);
    }

    [HttpPost("preview")]
    public async Task<PreviewResponse> Preview([FromBody] PreviewRequest request)
    {
        var method = BuildMethod(request);
        var content = _renderer.RenderController("Api", request.Prefix, request.ControllerName, new[] { method });
        var oldText = await _workspace.ReadFileAsync(request.TargetFileRelative);
        var diff = _diff.Diff(oldText, content);
        var files = new List<GeneratedFile> { new(request.TargetFileRelative, content) };
        return new PreviewResponse(diff, files);
    }

    [HttpPost("apply")]
    public async Task<object> Apply([FromBody] PreviewRequest request)
    {
        var method = BuildMethod(request);
        var content = _renderer.RenderController("Api", request.Prefix, request.ControllerName, new[] { method });
        await _workspace.WriteFileAsync(request.TargetFileRelative, content);
        return new { written = new[] { request.TargetFileRelative } };
    }

    private static MethodTemplate BuildMethod(PreviewRequest request) => new(
        request.Http,
        request.RouteTemplate,
        request.MethodName,
        request.ReturnType,
        request.Parameters.Select(p => new ParameterDescriptor(p.Type, p.Name)).ToList());
}
