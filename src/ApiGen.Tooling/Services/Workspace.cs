using System;
using System.IO;
using Microsoft.Extensions.Options;
using ApiGen.Tooling.Models;

namespace ApiGen.Tooling.Services;

public sealed class Workspace : IWorkspace
{
    private readonly string _root;

    public Workspace(IOptions<WorkspaceOptions> options)
    {
        _root = Path.GetFullPath(options.Value.Root);
    }

    public string Root => _root;

    private string Resolve(string relativePath)
    {
        var target = Path.GetFullPath(Path.Combine(_root, relativePath));
        var ok = target.StartsWith(_root + Path.DirectorySeparatorChar, StringComparison.Ordinal)
                 || string.Equals(target, _root, StringComparison.Ordinal);
        if (!ok) throw new InvalidOperationException("Path outside workspace");
        return target;
    }

    public bool Exists(string relativePath)
    {
        var path = Resolve(relativePath);
        return File.Exists(path);
    }

    public async Task<string?> ReadFileAsync(string relativePath)
    {
        var path = Resolve(relativePath);
        return File.Exists(path) ? await File.ReadAllTextAsync(path) : null;
    }

    public async Task WriteFileAsync(string relativePath, string content)
    {
        var path = Resolve(relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content);
    }
}
