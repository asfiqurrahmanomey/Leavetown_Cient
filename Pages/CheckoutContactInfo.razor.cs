using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Models;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using UriParser = Leavetown.Client.Components.Inline.UriParser;

namespace Leavetown.Client.Pages
{
    public partial class CheckoutContactInfo
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private IListingClient ListingClient { get; set; } = default!;
        [Inject] private IQuoteService QuoteService { get; set; } = default!;
        [Inject] private ILogger<CheckoutContactInfo> Logger { get; set; } = default!;
        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] private I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private BookingViewModel? _booking;

        private List<FormStepItem> _checkoutStepItems = new();
        private UriParser? _uriParser;
        private ListingDetailsViewModel? _listing;
        private QuoteModel? _quote;

        protected override void OnInitialized()
        {
            _checkoutStepItems = new List<FormStepItem>
            {
                new FormStepItem(0, title: ResourcesShared.ReviewYourStay, subtitle: ResourcesShared.ConfirmYourDetails),
                new FormStepItem(1, title: ResourcesShared.Info, subtitle: ResourcesShared.EnterYourDetails),
                new FormStepItem(2, title: ResourcesShared.Confirm, subtitle: ResourcesShared.ConfirmYourStay),
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (!firstRender) return;

                _booking ??= await _uriParser?.ParseQueryAsBookingAsync(new Uri(NavigationManager.Uri))!;

                if (_booking == null) throw new Exception("Failed to parse booking from URL query.");

                _listing = await LocalStorageService.GetStorageValueAsync<ListingDetailsViewModel>(StorageKey.Listing)!;
                _listing ??= await ListingClient.GetSingleListingDetailsModelAsync(_booking.ListingId);

                if (_listing == null) throw new Exception($"Failed to retrieve listing data for listing {_booking.ListingId}");

                await UpdateQuoteAsync();
            }
            catch
            {
                NavigationManager.NavigateTo($"{Culture}/pagenotfound");
                throw;
            }
        }

        private void OnContactDetailsSubmit()
        {
            try
            {
                if (_booking == null) return;

                // Dictionary of contact info query parameters 
                Dictionary<string, string> contactDetailsQueryParams = new()
                {
                    { nameof(BookingViewModel.FirstName).ToLower(), _booking.FirstName },
                    { nameof(BookingViewModel.LastName).ToLower(), _booking.LastName },
                    { nameof(BookingViewModel.Email).ToLower(), _booking.Email },
                    { nameof(BookingViewModel.PhoneNumber).ToLower(), _booking.PhoneNumber },
                    { nameof(BookingViewModel.ContactAddressDetails.Address1).ToLower(), _booking.ContactAddressDetails.Address1 },
                    { nameof(BookingViewModel.ContactAddressDetails.Address2).ToLower(), _booking.ContactAddressDetails.Address2 },
                    { nameof(BookingViewModel.ContactAddressDetails.ProvinceState).ToLower(), _booking.ContactAddressDetails.ProvinceState },
                    { nameof(BookingViewModel.ContactAddressDetails.City).ToLower(), _booking.ContactAddressDetails.City },
                    { nameof(BookingViewModel.ContactAddressDetails.PostalCode).ToLower(), _booking.ContactAddressDetails.PostalCode },
                    { "countrycode", _booking.ContactAddressDetails.Country.Code! }
                };    

                // Dictionary of all query parameters in current query
                Dictionary<string, string> bookingQueryDictionary = QueryHelpers.ParseQuery(_booking.Query).ToDictionary(x => x.Key, x => x.Value.ToString());

                // Dictionary of query parameters that exist in contact info query parameters dictionary that don't exist in current query
                Dictionary<string, string> newQueryValues = contactDetailsQueryParams
                    .UnionBy(bookingQueryDictionary, x => x.Key.ToLower())
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                    .ToDictionary(x => x.Key, x => x.Value);
               
                _booking.Query = QueryHelpers.AddQueryString(string.Empty, newQueryValues);
                var uriLeftPart = new Uri(NavigationManager.Uri).GetLeftPart(UriPartial.Path);
                var uri = new Uri($"{uriLeftPart}{_booking.Query}");               
                var path = $"{Culture}/checkout/confirm/{uri.Query}";
                NavigationManager.NavigateTo(path);
            }
            catch (Exception ex)
            {
                Logger.LogError("Booking query failed during construction: {Query} {Error}", _booking?.Query, ex.Message);
                throw;
            }
        }

        private async Task UpdateQuoteAsync()
        {
            _quote = await LocalStorageService.GetStorageValueAsync<QuoteModel>(StorageKey.Quote);

            if (_quote == null && _booking != null)
            {
                _quote = await QuoteService.UpdateQuoteAsync(
                    _booking.ListingId,
                    _booking,
                    _booking.CheckIn,
                    _booking.CheckOut,
                    _booking.NumberOfAdults,
                    _booking.NumberOfChildren,
                    _booking.NumberOfPets
                );

                await LocalStorageService.SetStorageValueAsync(StorageKey.Quote, _quote);
            }

            StateHasChanged();
        }
    }
}
