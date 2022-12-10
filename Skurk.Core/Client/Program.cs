using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Skurk.Core.Client;
using Skurk.Core.Client.State;
using Skurk.Core.Client.State.Store.Orders;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddSharedClientDependencies();
builder.Services.AddStatefulDependencies(builder.HostEnvironment);

builder.Services.AddFluxor(options => options.ScanAssemblies(Assembly.GetExecutingAssembly(), new Assembly[] { typeof(OrdersState).Assembly }));

await builder.Build().RunAsync();
