using Microsoft.JSInterop;

namespace Leavetown.Client.Utilities.Extensions
{
    public static class JSRuntimeExtensions
    {
        public static async Task SetDotNetReference<T>(this IJSRuntime jSRuntime, T reference) where T : class
        {
            var lDotNetReference = DotNetObjectReference.Create(reference);
            await jSRuntime.InvokeVoidAsync("GLOBAL.SetDotnetReference", lDotNetReference);
        }
    }
}
