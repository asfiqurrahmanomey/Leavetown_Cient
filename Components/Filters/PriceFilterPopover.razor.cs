using Leavetown.Client.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Filters;

public partial class PriceFilterPopover
{
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
    [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
    [Parameter] public PriceFilterModel? PriceValue { get; set; }
    private Popover? _pricePopover = new();
    
    public string FilterTitle => FilterRepresentationHelper.GetFilterRepresentation(PriceValue, ResourcesShared);
}