using System.Collections.Generic;

namespace ApiGen.Tooling.Models;

public sealed record MethodTemplate(
    string Http,
    string Template,
    string Name,
    string ReturnType,
    IReadOnlyList<ParameterDescriptor> Parameters);
