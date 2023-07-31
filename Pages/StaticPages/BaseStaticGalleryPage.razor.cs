using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Pages.StaticPages
{
    public abstract partial class BaseStaticGalleryPage
    {
        [Inject] private IJSRuntime? JSRuntime { get; set; }        

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && JSRuntime != null) await JSRuntime.InvokeVoidAsync("initGallery");
            await base.OnAfterRenderAsync(firstRender);
        }        
    }
}
