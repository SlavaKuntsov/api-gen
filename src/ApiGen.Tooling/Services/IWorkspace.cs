namespace ApiGen.Tooling.Services;

public interface IWorkspace
{
    string Root { get; }
    bool Exists(string relativePath);
    Task<string?> ReadFileAsync(string relativePath);
    Task WriteFileAsync(string relativePath, string content);
}
