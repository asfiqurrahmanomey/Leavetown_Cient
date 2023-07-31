using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants.MapBox;
using Leavetown.Client.Models.MapBox;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Leavetown.Client.Services
{
    public class MapBoxService : IMapBoxService
    {
        public LocationModel CurrentLocation { get; set; } = new(0M, 0M);

        private NavigationManager _navigationManager;
        private IJSRuntime _jsRuntime;
        private ICurrencyService _currencyService;
        private ICurrencyClient _currencyClient;
        private IExchangeRateService _exchangeRateService;
        private IPricingAvailabilityService _pricingAvailabilityService;

        public event EventHandler MapFilter = default!;

        public MapBoxService(
            NavigationManager navigationManager, 
            IJSRuntime jsRuntime, 
            ICurrencyService currencyService, 
            ICurrencyClient currencyClient,
            IExchangeRateService exchangeRateService,
            IPricingAvailabilityService pricingAvailabilityService)
        {
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _currencyService = currencyService;
            _currencyClient = currencyClient;
            _exchangeRateService = exchangeRateService;
            _pricingAvailabilityService = pricingAvailabilityService;
        }

        #region JavaScript Function Wrappers

        public async Task<string> GetInnerHtmlAsync(string id) => await _jsRuntime.InvokeAsync<string?>("getElementInnerHtml", id) ?? "";

        public async Task SetIdAsync(string oldId, string newId) => await _jsRuntime.InvokeVoidAsync("setId", oldId, newId);

        public async Task InitializeMapAsync()
        {
            await _jsRuntime.SetDotNetReference(this);
            await _jsRuntime.InvokeVoidAsync("initMap");
        }

        public async Task ClearMarkersAsync() => await _jsRuntime.InvokeVoidAsync("clearMarkers");

        public async Task SetMarkersAsync(List<GeoJson> data) => await _jsRuntime.InvokeVoidAsync("setMarkers", data);

        public async Task SetCircleAsync(List<GeoJson> data) => await _jsRuntime.InvokeVoidAsync("setCircle", data);

        public async Task SetCenterAsync(decimal latitude, decimal longitude, bool smoothTransition = true) => await _jsRuntime.InvokeVoidAsync("setCenter", latitude, longitude, smoothTransition);

        public async Task SetZoomLevelAsync(int zoomLevel) => await _jsRuntime.InvokeVoidAsync("setZoom", zoomLevel);

        public async Task FitToBoundsAsync(List<GeoJson> data, bool smoothTransition = true) => await _jsRuntime.InvokeVoidAsync("fitToBounds", data, smoothTransition);

        public async Task<string> GetBoundsAsync() => await _jsRuntime.InvokeAsync<string>("getBounds");

        public async Task SetFiltersAsync() => await _jsRuntime.InvokeVoidAsync("setFilterEvents");

        #endregion

        public async Task<GeoJson> ConvertSingleListingAsync(string id, decimal longitude, decimal latitude, GeometryType geometryType, string? label = null)
        {
            string content = await GetInnerHtmlAsync($"html-content-{id}");

            // This is needed to differentiate between the id of the element we set on the Destinations page and id of the element we inject into the description of the marker pop-up.
            //  Without this, the image carousel does not know which element to reference when cycling through images and functionality will break.
            await SetIdAsync($"imageCarousel-{id}-map", $"imageCarousel-{id}-map-placeholder");

            return new GeoJson(new List<double>() { (double)longitude, (double)latitude }, content, label, geometryType, id);
        }
       
        [JSInvokable(nameof(OnMapFilterEvent))]
        public void OnMapFilterEvent()
        {
            MapFilter?.Invoke(this, EventArgs.Empty);
        }
    }
}
