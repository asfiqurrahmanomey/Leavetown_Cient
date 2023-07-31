using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class Overlay
    {
        [Parameter] public Action CloseAction { get; set; } = () => { };
    }
}
