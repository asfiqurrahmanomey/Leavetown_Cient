using Leavetown.Client.Components.Filters;
using Leavetown.Client.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class GuestInfo
    {
        [Parameter] public int AdultCount { get; set; } = default!;
        [Parameter] public int ChildCount { get; set; } = default!;
        [Parameter] public int PetCount { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;


        private string GetGuestRepresentation()
        {
            GuestFilterModel guests = new()
            {
                AdultCount = AdultCount,
                ChildCount = ChildCount,
                GuestCount = AdultCount + ChildCount
            };

            PetFilterModel pets = new() { PetCount = PetCount };

            List<string> filterRepresentationStrings = new()
            {
                FilterRepresentationHelper.GetFilterRepresentation(guests, ResourcesShared)
            };

            if (PetCount > 0) filterRepresentationStrings.Add(FilterRepresentationHelper.GetFilterRepresentation(pets, ResourcesShared));

            return string.Join(", ", filterRepresentationStrings);
        }
    }
}
