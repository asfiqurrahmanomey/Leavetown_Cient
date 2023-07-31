using Leavetown.Client.Models.ViewModels;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Components;

public partial class EmblaCarousel
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Parameter] public List<RenderFragment> Slides { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeAsync<Task>("embla_setup");
        }
    }
}