using Leavetown.Client.Services.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

namespace Leavetown.Client.Utilities.Extensions
{
    public static class WebAssemblyHostExtension
    {
        public async static Task SetDefaultCulture(this WebAssemblyHost host)
        {
            var localStorage = host.Services.GetRequiredService<ILocalStorageService>();
            var result = await localStorage.GetStorageValueAsync<string>("culture");
            CultureInfo culture;
            if (result != null)
            {
                culture = new CultureInfo(result);
            }
            else
            {
                await localStorage.SetStorageValueAsync("culture", "en");
                culture = new CultureInfo("en");
            }
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
