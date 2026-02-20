using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProductCatalog.Blazor;
using ProductCatalog.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7223")
});
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<ProductsApi>();

await builder.Build().RunAsync();
