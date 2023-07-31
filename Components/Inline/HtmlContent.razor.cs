using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class HtmlContent
    {
        [Parameter] public RenderFragment ChildContent { get; set; } = default!;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
    }
}
