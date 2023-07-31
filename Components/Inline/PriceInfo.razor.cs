using Leavetown.Client.Utilities.Settings.Contracts;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class PriceInfo
    {
        [Inject] private IRenderState RenderState { get; set; } = default!;
        [Parameter] public string Label { get; set; } = default!;
        [Parameter] public bool IsAverage { get; set; } = true;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;


        private string GetPriceString()
        {
            if (RenderState.IsPrerender) return ResourcesShared.LoadingPrice;
            if (string.Equals(Label, ResourcesShared.InquireOnly)) return Label;

            return string.Format(IsAverage ? ResourcesShared.AvgPricePerNight : ResourcesShared.BookNowForPrice, new object[] { Label });
        }
    }
}
