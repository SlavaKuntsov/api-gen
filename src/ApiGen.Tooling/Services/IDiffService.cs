namespace ApiGen.Tooling.Services;

public interface IDiffService
{
    string Diff(string? oldText, string newText);
}
