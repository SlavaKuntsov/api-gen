using System.Collections.Generic;

namespace ApiGen.Web.Models;

public sealed record GeneratedFile(string Path, string Content);

public sealed record PreviewResponse(string Diff, IReadOnlyList<GeneratedFile> Files);
