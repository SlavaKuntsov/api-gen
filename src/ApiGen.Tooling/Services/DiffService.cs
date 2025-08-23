using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace ApiGen.Tooling.Services;

public sealed class DiffService : IDiffService
{
    public string Diff(string? oldText, string newText)
    {
        var builder = new InlineDiffBuilder(new Differ());
        var model = builder.BuildDiffModel(oldText ?? string.Empty, newText);
        var sb = new StringBuilder();
        foreach (var line in model.Lines)
        {
            var prefix = line.Type switch
            {
                ChangeType.Inserted => "+",
                ChangeType.Deleted => "-",
                ChangeType.Imaginary => " ",
                _ => " "
            };
            sb.Append(prefix);
            sb.AppendLine(line.Text);
        }
        return sb.ToString();
    }
}
