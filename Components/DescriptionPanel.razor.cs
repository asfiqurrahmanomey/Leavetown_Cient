using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components;

public partial class DescriptionPanel
{
    [Parameter] public string Description { get; set; } = "";
    [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
    [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
    private bool _descriptionExpanded = false;

    private void ToggleDescriptionExpand() => _descriptionExpanded = !_descriptionExpanded;
}