using System.Collections.Generic;

namespace ApiGen.Web.Models;

public sealed class PreviewRequest
{
    public required string ControllerName { get; init; }
    public required string Prefix { get; init; }
    public required string Http { get; init; }
    public required string RouteTemplate { get; init; }
    public required string MethodName { get; init; }
    public required List<ParameterModel> Parameters { get; init; }
    public required string ReturnType { get; init; }
    public required string TargetFileRelative { get; init; }
    public required string InsertMode { get; init; }
}
