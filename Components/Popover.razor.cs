using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Components
{
    public partial class Popover
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Parameter] public string Id { get; set; } = default!;
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter] public string Placeholder { get; set; } = default!;
        [Parameter] public string Title { get; set; } = default!;        
        [Parameter] public bool FullScreenOnMobile { get; set; } = true;
        [Parameter] public bool IsCentered { get; set; } = true;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;


        public void ToggleCardVisibility()
        {            
            InvokeAsync(async () => await JSRuntime.InvokeVoidAsync("togglePopover", Id));
            StateHasChanged();
        }
    }
}
