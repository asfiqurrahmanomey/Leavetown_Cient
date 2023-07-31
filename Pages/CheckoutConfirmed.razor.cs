using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Text;
using Leavetown.Client.Constants;
using Leavetown.Client.Utilities.Settings.Contracts;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Clients;
using Leavetown.Client.Clients.Contracts;
using UriParser = Leavetown.Client.Components.Inline.UriParser;

namespace Leavetown.Client.Pages
{
    public partial class CheckoutConfirmed
    {
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IListingClient ListingClient { get; set; } = default!;
        [Inject] private IQuoteService QuoteService { get; set; } = default!;

        [Parameter] public string? Culture { get; set; }
        [Parameter] public string BookingId { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private BookingViewModel? _booking;
        private QuoteModel? _quote;
        private string? _partyDetails;
        private UriParser? _uriParser;
        private ListingDetailsViewModel? _listing;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            _booking ??= await _uriParser?.ParseQueryAsBookingAsync(new Uri(NavigationManager.Uri))!;

            if (_booking == null) throw new Exception("Failed to parse booking from URL query.");

            _listing = await LocalStorageService.GetStorageValueAsync<ListingDetailsViewModel>(StorageKey.Listing)!;
            _listing ??= await ListingClient.GetSingleListingDetailsModelAsync(_booking.ListingId);

            if (_listing == null) throw new Exception($"Failed to retrieve listing data for listing {_booking.ListingId}");

            _partyDetails = BuildPartyDetailsString();
            await UpdateQuoteAsync();
        }

        private string BuildPartyDetailsString()
        {
            List<string> partyDetailsStrings = new();

            var numberOfAdultsString = _booking!.NumberOfAdults > 1
                ? ResourcesShared.NumberOfAdultsPlural
                : ResourcesShared.NumberOfAdultsSingular;
            partyDetailsStrings.Add(string.Format(numberOfAdultsString, _booking!.NumberOfAdults));

            if (_booking.NumberOfChildren > 0)
            {
                var numberOfChildrenString = _booking!.NumberOfChildren > 1
                    ? ResourcesShared.NumberOfChildrenPlural
                    : ResourcesShared.NumberOfChildrenSingular;
                partyDetailsStrings.Add(string.Format(numberOfChildrenString, _booking.NumberOfChildren));
            }

            if (_booking.NumberOfPets > 0)
            {
                var numberOfPetsString = _booking!.NumberOfPets > 1
                    ? ResourcesShared.NumberOfPetsPlural
                    : ResourcesShared.NumberOfPetsSingular;
                partyDetailsStrings.Add(string.Format(numberOfPetsString, _booking.NumberOfPets));
            }

            return string.Join(", ", partyDetailsStrings);
        }

        private async Task UpdateQuoteAsync()
        {
            if (_booking == null) throw new ArgumentNullException("Cannot update Quote. Booking is null.");

            _quote = await QuoteService.UpdateQuoteAsync(
                _booking.ListingId,
                _booking,
                _booking.CheckIn,
                _booking.CheckOut,
                _booking.NumberOfAdults,
                _booking.NumberOfChildren,
                _booking.NumberOfPets
            );

            StateHasChanged();
        }

        private void OnBackToSearchButtonClicked()
        {
            NavigationManager.NavigateTo($"{Culture}/accommodations");
        }
    }
}