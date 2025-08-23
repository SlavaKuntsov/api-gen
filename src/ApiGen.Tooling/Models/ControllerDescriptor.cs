using System.Collections.Generic;

namespace ApiGen.Tooling.Models;

public sealed record ControllerDescriptor(
    string Controller,
    string Prefix,
    IReadOnlyList<ActionDescriptor> Actions);
