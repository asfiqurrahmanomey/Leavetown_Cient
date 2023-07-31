using Microsoft.AspNetCore.Components;
using Radzen;
using Leavetown.Client.Components.Filters;
using Leavetown.Client.Components.Forms;
using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Client.Constants;
using Leavetown.Client.Utilities;
using Leavetown.Client.Utilities.Settings.Contracts;
using Leavetown.Shared.Models.ViewModels.Contracts;
using Microsoft.Extensions.Logging;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Client.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Leavetown.Shared.Models.PartnerApi;

namespace Leavetown.Client.Components
{
    public partial class BookingPanel
    {
        [Inject] private DialogService DialogService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private IPricingAvailabilityService PricingAvailabilityService { get; set; } = default!;
        [Inject] private IQuoteService QuoteService { get; set; } = default!;
        [Inject] private IRenderState RenderState { get; set; } = default!;

        [Parameter] public string Id { get; set; } = "";
        [Parameter] public ListingDetailsViewModel Listing { get; set; } = new();
        [Parameter] public AvailabilityFilterModel AvailabilityValue { get; set; } = new();
        [Parameter] public GuestFilterModel GuestValue { get; set; } = new();
        [Parameter] public PetFilterModel PetValue { get; set; } = new();
        [Parameter] public EventCallback<AvailabilityFilterModel> AvailabilityValueChanged { get; set; } = default!;
        [Parameter] public Action<AvailabilityFilterModel> AvailabilityChanged { get; set; } = (args) => { };
        [Parameter] public Action<bool> BookingPanelVisibilityChanged { get; set; } = (args) => { };
        [Parameter] public string PriceLabel { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private Debouncer _debouncer = new();
        private ListingAvailabilityFilter? _bookingListingAvailabilityFilter;
        private GuestFilter? _bookingGuestsFilter;

        private QuoteModel? _quote;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _bookingGuestsFilter?.GuestChanged.Invoke(GuestValue);
                StateHasChanged();
            }
        }

        protected override async Task OnParametersSetAsync() => await _debouncer.Debounce(async () => 
        {
            if (ValidateBookingData())
            {
                await UpdateQuoteAsync();
                StateHasChanged();
            }

            await base.OnParametersSetAsync();
        });

        private void GetNextBookingStep()
        {
            if (!AvailabilityValue.HasValue)
            {
                _bookingListingAvailabilityFilter?.Expand();
            }
            else
            {
                if (_quote == null) throw new Exception("Quote value is null. API may have failed to return a value.");

                InvokeAsync(async () => {
                    await LocalStorageService.SetStorageValueAsync(StorageKey.Quote, _quote);
                    await LocalStorageService.SetStorageValueAsync(StorageKey.Listing, Listing);
                    string query = new Uri(NavigationManager.GetUriWithQueryParameter(nameof(BookingViewModel.ListingId).ToLower(), Listing.Id)).Query;
                    NavigationManager.NavigateTo($"{await CultureService.GetCultureAsync()}/checkout{query}");
                });
            }
        }

        private void OpenInquiryDialog()
        {
            var propertyId = Listing.Id;

            DialogService.Open<InquiryForm>(ResourcesShared.MakeAnInquiry,
                   new Dictionary<string, object>() { 
                       { nameof(InquiryForm.Availability), AvailabilityValue },
                       { nameof(InquiryForm.PropertyId), propertyId }
                   },
                   new DialogOptions() { CloseDialogOnOverlayClick = true });
        }

        public void SetBookingDateValue(AvailabilityFilterModel? availability) => _bookingListingAvailabilityFilter?.Set(availability);

        private void OnGuestChanged(GuestFilterModel guests)
        {
            GuestValue.GuestCount = guests.GuestCount > 0 ? guests.GuestCount : 1;
            GuestValue.AdultCount = guests.AdultCount > 0 ? guests.AdultCount : 1;
            GuestValue.ChildCount = guests.ChildCount;

            if (ValidateBookingData()) InvokeAsync(async () => {
                await _debouncer.Debounce(async () => await UpdateQuoteAsync());
                StateHasChanged();
            });
        }

        private void OnPetChanged(PetFilterModel pets)
        {
            if (Listing == null || !Listing.PetsAllowed) return;

            PetValue.PetCount = pets.PetCount;

            if (ValidateBookingData()) InvokeAsync(async () => {
                await _debouncer.Debounce(async () => await UpdateQuoteAsync());
                StateHasChanged();
            });
        }

        private void OnDateChanged(AvailabilityFilterModel availability)
        {
            AvailabilityChanged.Invoke(availability);

            if (ValidateBookingData()) InvokeAsync(async () => {
                await _debouncer.Debounce(async () => await UpdateQuoteAsync());
                StateHasChanged();
            });
        }

        private void HideBookingPanel()
        {
            BookingPanelVisibilityChanged.Invoke(false);
        }

        private async Task UpdateQuoteAsync()
        {
            _quote = null;
            StateHasChanged();

            _quote = await QuoteService.UpdateQuoteAsync(
                Listing.Id,
                Listing,
                AvailabilityValue.Start,
                AvailabilityValue.End,
                GuestValue.AdultCount,
                GuestValue.ChildCount,
                PetValue.PetCount
            );

            StateHasChanged();
        }

        private bool ValidateBookingData() => 
            Listing != null && 
            AvailabilityValue.HasValue && 
            GuestValue.HasValue;

        private void OnStartDateSelected(DateTimeOffset offset)
        {
            StateHasChanged();
        }
    }
}
