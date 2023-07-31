using Microsoft.AspNetCore.Components;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Components;
using Leavetown.Client.Components.Filters;
using Leavetown.Client.Constants.MapBox;
using Leavetown.Client.Models.MapBox;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Constants;
using Microsoft.AspNetCore.Components.Routing;
using Leavetown.Client.Response;

namespace Leavetown.Client.Pages
{
    public partial class ListingDetails
    {
        [Inject] private IListingClient ListingClient { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IMapBoxService MapBoxService { get; set; } = default!;
        [Inject] private IPricingAvailabilityService PricingAvailabilityService { get; set; } = default!;
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] private ILogger<ListingDetails> Logger { get; set; } = default!;
        [Inject] private IResponse? _response { get; set; }

        [Parameter] public int ListingId { get; set; }
        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;

        private const string ListingKey = "listing";

        private bool _isScreenLarge = false;

        private ListingDetailsViewModel? _listing;
        private List<PricingAvailabilityModel>? _pricingAvailabilities;

        private BookingPanel? _bookingPanel;
        private ListingAvailabilityFilter? _listingAvailabilityFilter;

        private AvailabilityFilterModel _availabilityValue = new();
        private bool _bookingWindowDisplayed = false;

        private List<GeoJson> _mapBoxData = new();

        private CurrencyModel? _localCurrency;
        private CurrencyModel? _listingCurrency;
        private string _priceLabel = "";        
        private ILookup<bool, AmenityModel>? _amenitiesLookUp;
        private List<AmenityModel>? _amenities;
        private List<AmenityModel>? _suitabilities;
        private Map? _map;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            CurrencyService.CurrencyChanged += OnCurrencyChanged;
            await InitializeDataAsync();
        }
        
        private async Task InitializeDataAsync()
        {
            try
            {
                _listing = await GetPageModel(async () => await ListingClient.GetSingleListingDetailsModelAsync(ListingId));

                if (_listing == null)
                {
                    Logger.LogWarning("Failed to retrieve listing details {ListingId}", ListingId);
                }
                else
                {
                    _amenitiesLookUp = _listing?.Amenities?.ToLookup(x => x.AmenityCategoryId == AmenityCategory.Suitability.Id);

                    if (_amenitiesLookUp != null)
                    {
                        _amenities = _amenitiesLookUp[false].ToList();
                        _suitabilities = _amenitiesLookUp[true].ToList();
                    }

                    _pricingAvailabilities = PricingAvailabilityService.GetPricingAvailabilitiesBetweenDates(_listing, _availabilityValue)?.ToList();
                    await UpdatePricingLabelAsync();
                    await InitializeMapAsync();
                }

                _isLoading = false;
            }
            // exclude NavigationException as there is a known issue with this exception incorrectly getting thrown when calling
            // NavigateTo within OnInitialized
            catch (Exception ex) when (ex is not NavigationException)
            {
                Logger.LogWarning(ex, "Failed to retrieve listing details {ListingId}: Exception: {Exception}", ListingId, ex.Message);

                // if the listing no longer exists or is inactive, just redirect back to the home page
                NavigationManager.NavigateTo($"{NavigationManager.BaseUri}{Culture}/");
            }
        }
        
        private async Task UpdatePricingLabelAsync()
        {
            if (_listing == null) return;

            _localCurrency = await CurrencyService.GetLocalCurrencyAsync();
            if (_localCurrency == null) return;
            if (_listing.CurrencyCode == null) return;

            _listingCurrency = CurrencyService.GetCurrencyFromCode(_listing.CurrencyCode);
            if (_listingCurrency == null) return;

            _priceLabel = await PricingAvailabilityService.GetAverageCostOfStayLabelAsync(_listing.PricingAvailabilities, _listingCurrency, _localCurrency, _availabilityValue, ResourcesShared.InquireOnly);
            StateHasChanged();
        }

        private async Task InitializeMapAsync()
        {
            if (_map == null) return;

            // TODO: Handle listing not having Lat/Long. Should this ever happen?
            if (_listing?.Latitude == null || _listing?.Longitude == null) throw new ArgumentNullException($"Listing latitude and/or longitude is null. Latitude: {(_listing?.Latitude == null ? "null" : _listing.Latitude)}, Longitude: {(_listing?.Longitude == null ? "null" : _listing.Longitude)}");

            _mapBoxData.Add(await MapBoxService.ConvertSingleListingAsync(_listing.Id.ToString(), _listing.Longitude.Value, _listing.Latitude.Value, GeometryType.Circle));            
            
            await _map.UpdateMapAsync(_mapBoxData);
        }

        protected override void OnParametersSet()
        {
            _listingAvailabilityFilter?.Set(_availabilityValue);
            StateHasChanged();
        }
        
        public void DisplayBookingWindow() => _bookingWindowDisplayed = !_bookingWindowDisplayed;

        private void OnAvailabilityChanged(AvailabilityFilterModel availabilityValue)
        {
            _availabilityValue = availabilityValue;
            _listingAvailabilityFilter?.Set(_availabilityValue);
            _bookingPanel?.SetBookingDateValue(_availabilityValue);
            InvokeAsync(async () => await UpdatePricingLabelAsync());
        }

        private void OnBookingPanelVisibilityChanged(bool isVisible)
        {
            _bookingWindowDisplayed = isVisible;
            StateHasChanged();
        }
        
        private void OnStartDateSelected()
        {
            StateHasChanged();
        }

        private void OnCurrencyChanged(object? sender, Models.Events.CurrencyChangedEventArgs e)
        {
            _localCurrency = e.CurrencyModel;
            InvokeAsync(async () => await UpdatePricingLabelAsync());
        }
        
        public override void Dispose()
        {
            CurrencyService.CurrencyChanged -= OnCurrencyChanged;
            base.Dispose();
        }
    }
}