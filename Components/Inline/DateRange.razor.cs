using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class DateRange
    {
        [Parameter] public DateTime Checkin { get; set; }
        [Parameter] public DateTime Checkout { get; set; }
        [Parameter] public bool IncludeYear { get; set; }
    }
}
