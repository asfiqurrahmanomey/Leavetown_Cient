using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Pages
{
    public partial class StripePaymentResponse
    {
        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;


    }
}
