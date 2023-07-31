using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class SocialLinks
    {
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
    }
}
