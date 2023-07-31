using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Pages.StaticPages
{
    public partial class TermsAndConditions
    {
        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;

    }
}
