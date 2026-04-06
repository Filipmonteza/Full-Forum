using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FullForum_WebUi;
using FullForum_WebUi.Services;
using FullForum_WebUi.Services.Auth;
using FullForum_WebUi.Services.UI;
using FullForum_WebUiWebUi.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["WebApi:BaseUrl"]
                 ?? "https://localhost:5085";

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

builder.Services.AddAuthorizationCore();

builder.Services
    .AddScoped<AuthState>()
    .AddScoped<IUiStatus, UiStatus>()
    .AddScoped<ITokenStore, TokenStore>()
    .AddScoped<ApiClient>();

await builder.Build().RunAsync();