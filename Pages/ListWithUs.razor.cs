using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Pages
{
    public partial class ListWithUs
    {
        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
    }
}
