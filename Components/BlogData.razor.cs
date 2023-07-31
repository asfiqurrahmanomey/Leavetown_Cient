using Leavetown.Shared.Models.LeavetownCms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Components
{
    public partial class BlogData
    {
        [Parameter] public BlogPostModel? Blog { get; set; }
        [Parameter] public string Culture { get; set; } = "";
        [Parameter] public string AdditionalBlogCaption { get; set; } = string.Empty;
        [Parameter] public bool IsTop { get; set; } = false;
        [Parameter] public bool ShowButton { get; set; } = true;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("instgrm.Embeds.process");
        }
    }
}
