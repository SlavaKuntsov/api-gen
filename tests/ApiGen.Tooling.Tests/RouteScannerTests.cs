using System.IO;
using System.Threading.Tasks;
using ApiGen.Tooling.Services;
using Xunit;

namespace ApiGen.Tooling.Tests;

public class RouteScannerTests
{
    [Fact]
    public async Task Finds_Controller_Actions()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        var code = """
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();
}
""";
        var file = Path.Combine(dir, "TestController.cs");
        await File.WriteAllTextAsync(file, code);
        var scanner = new RouteScanner();
        var controllers = await scanner.ScanAsync(dir);
        Assert.Single(controllers);
        Assert.Equal("TestController", controllers[0].Controller);
        Assert.Single(controllers[0].Actions);
        Assert.Equal("GET", controllers[0].Actions[0].Http);
    }

    [Fact]
    public async Task Ignores_NonController()
    {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        var code = """
public class Foo {}
""";
        var file = Path.Combine(dir, "Foo.cs");
        await File.WriteAllTextAsync(file, code);
        var scanner = new RouteScanner();
        var controllers = await scanner.ScanAsync(dir);
        Assert.Empty(controllers);
    }
}
