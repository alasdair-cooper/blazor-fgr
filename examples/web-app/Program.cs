using BlazorFgr.Core;
using BlazorFgr.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents(x => x.DetailedErrors = true);

builder.Services.AddBlazorFgr(x => x.AreDiagnosticsEnabled = true);

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();