using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Pages
{
    public partial class Contact
    {
        [Parameter] public string? Culture { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;

        public async Task ShowChat()
        {
            await JSRuntime.InvokeVoidAsync("FrontChat", "show");
        }

        private string GetSecondaryPhoneTypePath()
        {
            return $"images/whitelabel/{ResourcesMicroSiteSpecific.SecondaryContactPhoneType ?? "phone"}_icon.svg";
        }
    }
}
