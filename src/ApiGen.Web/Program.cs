using System.IO;
using ApiGen.Tooling.Models;
using ApiGen.Tooling.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.Configure<WorkspaceOptions>(builder.Configuration.GetSection("Workspace"));
builder.Services.AddSingleton<IWorkspace, Workspace>();
builder.Services.AddSingleton<IRouteScanner, RouteScanner>();
builder.Services.AddSingleton<IDiffService, DiffService>();
builder.Services.AddSingleton<ITemplateRenderer>(sp =>
    new TemplateRenderer(Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "..", "templates"))));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/Routes"));

app.Run();
