using Leavetown.Client.Models;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.PartnerApi;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels.Contracts;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class CheckoutPanel
    {
        [Parameter, EditorRequired] public ListingDetailsViewModel Listing { get; set; } = new();
        [Parameter, EditorRequired] public QuoteModel Quote { get; set; } = new();
        [Parameter, EditorRequired] public BookingViewModel Booking { get; set; } = new(); 

        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
    }
}
