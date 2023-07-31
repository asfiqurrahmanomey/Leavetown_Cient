using Leavetown.Client.Clients;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Models;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using UriParser = Leavetown.Client.Components.Inline.UriParser;

namespace Leavetown.Client.Pages
{
    public partial class CheckoutPayment
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private IQuoteService QuoteService { get; set; } = default!;
        [Inject] private IRenderState RenderState { get; set; } = default!;
        [Inject] private IListingClient ListingClient { get; set; } = default!; 
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

                _listing = await LocalStorageService.GetStorageValueAsync<ListingDetailsViewModel>(StorageKey.Listing)!;
                _listing ??= await ListingClient.GetSingleListingDetailsModelAsync(_booking.ListingId);

                await UpdateQuoteAsync();
            }
            catch
            {
                NavigationManager.NavigateTo($"{Culture}/pagenotfound");
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
