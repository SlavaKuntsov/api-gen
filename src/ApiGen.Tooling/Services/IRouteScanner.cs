using ApiGen.Tooling.Models;

namespace ApiGen.Tooling.Services;

public interface IRouteScanner
{
    Task<IReadOnlyList<ControllerDescriptor>> ScanAsync(string root);
}
