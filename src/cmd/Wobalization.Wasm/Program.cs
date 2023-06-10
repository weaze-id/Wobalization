using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RestSharp;
using Wobalization.Wasm;
using Wobalization.Wasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddValidatorsFromAssembly(Assembly.Load("Shared"));

builder.Services
    .AddHttpClient("API", options => options.BaseAddress = new Uri("http://localhost:5106"))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped(services =>
{
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    return new RestClient(httpClientFactory.CreateClient("API"));
});

builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<HttpMessageService>();

await builder.Build().RunAsync();