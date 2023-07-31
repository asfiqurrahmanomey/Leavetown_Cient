using Leavetown.Client.Components.Filters.Contracts;
using Leavetown.Client.Models;
using Microsoft.AspNetCore.Components;
using Leavetown.Client.Utilities.Extensions;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.WebUtilities;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Constants;

namespace Leavetown.Client.Components.Filters
{
    public partial class LocationFilter : IFilterComponent, IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ILocationFilterService LocationFilterService { get; set; } = default!;
        [Inject] private IDestinationService DestinationService { get; set; } = default!;

        [Parameter] public LocationFilterModel LocationValue { get; set; } = new();
        [Parameter] public EventCallback<LocationFilterModel> LocationValueChanged { get; set; } = default!;
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private Popover _locationPopover = new();
        private DrillDownMenu<DestinationModel> _drillDownMenu = new();
        private List<DestinationModel>? _destinations = new();
        private bool _disposedValue;

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InitializeDestinationDataAsync();
            }
        }

        private async Task InitializeDestinationDataAsync()
        {
            _destinations = await DestinationService.GetDestinationsAsync();
            StateHasChanged();
            _drillDownMenu.SetLevel(1);
            await UpdateFromUrlAsync(NavigationManager.Uri);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => InvokeAsync(async () => await UpdateFromUrlAsync(e.Location));

        private async Task UpdateFromUrlAsync(string uri)
        {
            if (uri == null) return;

            Dictionary<string, StringValues>? queryValues = QueryHelpers.ParseQuery(new Uri(uri).Query);
            queryValues.TryGetValue(nameof(FilterType.Location).ToLower(), out var value);
            LocationFilterModel location = value != StringValues.Empty ?
                LocationFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Location).ToLower(), value)) :
                new LocationFilterModel();

            if (!location.Equals(LocationValue))
            {
                location.Name = GetDenormalizedLocationName(location.Name);
                LocationValue = location;
                if (LocationValue.IsFilteringViaMap) LocationValue.Name = "map view";

                await LocationValueChanged.InvokeAsync(LocationValue);
                StateHasChanged();
            }
        }

        public void Reset(FilterType? filterType = null)
        {
            LocationValue = new();
            _drillDownMenu.Reset();
            StateHasChanged();
        }

        public void Expand() => _locationPopover.ToggleCardVisibility();

        private Task OnLocationValueChanged()
        {
            LocationValue = string.IsNullOrEmpty(_drillDownMenu?.Value) ? new LocationFilterModel() : new LocationFilterModel
            {
                Name = _drillDownMenu.Value,
            };

            NavigationManager.AddQueryParameter(LocationValue);
            return LocationValueChanged.InvokeAsync(LocationValue);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NavigationManager.LocationChanged -= OnLocationChanged;
                }

                _disposedValue = true;
            }
        }

        private string GetDenormalizedLocationName(string locationValue)
        {

            if (_destinations == null)
                return locationValue;

            var normalizedLocationValue = locationValue.ToLower();

            foreach (var destination in _destinations)
            {
                if (destination.RegionName.ToLower() == normalizedLocationValue)
                    return destination.RegionName;
                if (destination.CountryName.ToLower() == normalizedLocationValue)
                    return destination.CountryName;
                if (destination.DestinationName.ToLower() == normalizedLocationValue)
                    return destination.DestinationName;
            }

            return locationValue;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
