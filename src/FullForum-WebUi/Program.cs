using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FullForum_WebUi.Components.Shared;
using FullForum_WebUi.Services;
using FullForum_WebUi.Services.Auth;
using FullForum_WebUi.Services.UI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["WebApi:BaseUrl"]
                 ?? "https://localhost:7058";

builder.Services.AddScoped(_ => new HttpClient
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