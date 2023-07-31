using Leavetown.Client.Utilities.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Leavetown.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

await ServiceRegistry.RegisterServices(builder);

var host = builder.Build();

await host.SetDefaultCulture();

await host.RunAsync();
