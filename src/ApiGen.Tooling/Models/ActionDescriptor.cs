using System.Collections.Generic;

namespace ApiGen.Tooling.Models;

public sealed record ActionDescriptor(
    string Http,
    string Template,
    string Method,
    IReadOnlyList<ParameterDescriptor> Parameters);
