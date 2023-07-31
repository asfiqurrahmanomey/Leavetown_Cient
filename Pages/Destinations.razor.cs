using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Components;
using Leavetown.Client.Components.Filters;
using Leavetown.Client.Constants.MapBox;
using Leavetown.Client.Models;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Models.MapBox;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.Contracts;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Radzen;

namespace Leavetown.Client.Pages
{
    public partial class Destinations : IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private DialogService DialogService { get; set; } = default!;
        [Inject] private IListingClient ListingClient { get; set; } = default!;
        [Inject] private IPricingAvailabilityClient PricingAvailabilityClient{ get; set; } = default!;
        [Inject] private IMapBoxService MapBoxService { get; set; } = default!;
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] private ILogger<Destinations> Logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private ILocationFilterService LocationFilterService { get; set; } = default!;
        [Inject] private IGuestFilterService GuestFilterService { get; set; } = default!;
        [Inject] private IPetFilterService PetFilterService { get; set; } = default!;
        [Inject] private IPriceFilterService PriceFilterService { get; set; } = default!;
        [Inject] private IAvailabilityFilterService AvailabilityFilterService { get; set; } = default!;
        [Inject] private ISortingService SortingService { get; set; } = default!;
        [Inject] private IAmenitiesFilterService AmenitiesFilterService { get; set; } = default!;
        [Inject] private IBedroomFilterService BedroomFilterService { get; set; } = default!;
        [Inject] private IBathroomFilterService BathroomFilterService { get; set; } = default!;
        [Inject] private IBedFilterService BedFilterService { get; set; } = default!;
        [Inject] private IPropertyTypeFilterService PropertyTypeFilterService { get; set; } = default!;
        [Inject] private IPricingAvailabilityService PricingAvailabilityService { get; set; } = default!;

        [Parameter] public string? Culture { get; set; }
        [Parameter] public string? LocationShortcut { get; set; }
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;

        [CascadingParameter] public Configuration Configuration { get; set; } = default!;
        [CascadingParameter(Name = "RouteData")] public RouteData? RouteData { get; set; } = default!;

        private int _listingCount = 0;
        private List<ListingViewModel> _filteredListings = new();
        private LocationFilterModel _locationValue = new();
        private AvailabilityFilterModel _availabilityValue = new();
        private GuestFilterModel _guestValue = new();
        private PetFilterModel _petValue = new();

        private int _skipIncrement = 12;
        private int _top = 0;
        private int _skip = 0;
        private int _currentPage = 0;
        private int _numberOfPages = 0;
        private bool _isUpdating = false;
        private bool _isLoading = true;
        private Debouncer _debouncer = new();
        private Dictionary<string, IFilterable> _filters = new();
        private Dictionary<int, IFilterable> _appliedFilters = new();                                                                   
        private List<GeoJson> _mapBoxData = new();
        private Map? _map;
        private bool _hasFilterChanged = true;
        private bool _disposedValue;
        private bool _mapVisible = true;
        private bool _shouldRender = true;
        private Dictionary<int, string> _priceLabelDictionary = new();

        private string _currentUrl = "";

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            CurrencyService.CurrencyChanged += OnCurrencyChanged;

            if (!Configuration.WhiteLabelSettings.AccommodationsPageShowMap) _mapVisible = false;

            _top = _skipIncrement;
            InitializeFilters();
            _isLoading = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                if (Configuration.WhiteLabelSettings.AccommodationsPageShowMap)
                {
                    await RegisterResizeHandler();
                }
                await UpdateFiltersAsync(NavigationManager.Uri);
            }
            if (!firstRender && _hasFilterChanged)
            {
                if (Configuration.WhiteLabelSettings.AccommodationsPageShowMap) { 
                    await UpdateMapAsync();
                }
                _hasFilterChanged = false;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override bool ShouldRender() => _shouldRender;

        private void ToggleMap()
        {
            _mapVisible = !_mapVisible;
        }

        private async Task RegisterResizeHandler()
        {
            await JSRuntime.SetDotNetReference(this);
            var dotNetObjRef = DotNetObjectReference.Create(this);
            await JSRuntime.InvokeVoidAsync("registerOnResizeHandler", dotNetObjRef);
        }

        private void ShowMoreFilters()
        {
            DialogService.OpenAsync<AdditionalFilters>(ResourcesShared.MoreFilters);
        }

        [JSInvokable]
        public void ToggleMap(bool isVisible)
        {
            _mapVisible = isVisible;
            StateHasChanged();
        }

        private void InitializeFilters()
        {
            _filters = new Dictionary<string, IFilterable>
            {
                {
                    nameof(FilterType.Location).ToLower(),
                    new LocationFilterModel()
                },
                {
                    nameof(FilterType.Availability).ToLower(),
                    new AvailabilityFilterModel()
                },
                {
                    nameof(FilterType.Guests).ToLower(),
                    new GuestFilterModel()
                },                
                {
                    nameof(FilterType.Pets).ToLower(),
                    new PetFilterModel()
                },
                {
                    nameof(FilterType.SortBy).ToSnakeCase(),
                    new SortingOptions()
                },
                {
                    nameof(FilterType.Price).ToLower(),
                    new PriceFilterModel()
                },
                {
                    nameof(FilterType.Bathroom).ToSnakeCase(),
                    new BathroomFilterModel()
                },
                {
                    nameof(FilterType.Bedroom).ToSnakeCase(),
                    new BedroomFilterModel()
                },
                {
                    nameof(FilterType.Amenities).ToSnakeCase(),
                    new AmenitiesFilterModel()
                },
                {
                    nameof(FilterType.Beds).ToLower(),
                    new BedFilterModel()
                },
                {
                    nameof(FilterType.PropertyType).ToSnakeCase(),
                    new PropertyTypeFilterModel()
                },
            };
        }

        private async Task UpdateDataAsync(bool reset = false)
        {
            try
            {
                _isUpdating = true;
                StateHasChanged();

                _shouldRender = false;
                var filterQuery = await FilterQuery();
                
                _listingCount = (await ListingClient.GetListingModelsCountAsync(filterQuery) ?? 0);
                var listingCalculation = _listingCount + _skipIncrement - 1;
                _numberOfPages = listingCalculation > 0 && _skipIncrement > 0 ? listingCalculation / _skipIncrement : 0;
                if (reset) UpdatePagingParameters(1);

                _filteredListings = (await ListingClient.GetListingModelsAsync(_skip, _top, where: filterQuery, order: SortQuery()) 
                    ?? Enumerable.Empty<ListingViewModel>()).ToList();
                
                await UpdatePricingLabelsAsync(_filteredListings);

                _hasFilterChanged = true;
                _shouldRender = true;
            }
            catch (OperationCanceledException ex)
            {
                Logger.LogError(ex, "Error occurred while cancelling a JSInterop action");
            }
            catch (JSException ex)
            {
                Logger.LogError(ex, "Error occurred while executing a JavaScript function");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while executing");
            }
            finally
            {
                _isUpdating = false;
                StateHasChanged();
            }
        }

        private async Task UpdatePricingLabelsAsync(IEnumerable<ListingViewModel>? prices)
        {
            _priceLabelDictionary.Clear();

            if (prices == null) return;

            CurrencyModel? toCurrency = await CurrencyService.GetLocalCurrencyAsync();

            if (toCurrency == null) return;

            foreach (ListingViewModel listing in prices)
            {
                if (listing.CurrencyCode == null) continue;

                CurrencyModel? fromCurrency = CurrencyService.GetCurrencyFromCode(listing.CurrencyCode);
                if (fromCurrency == null) continue;

                bool useAverageForPrice = !_availabilityValue.HasValue || (!_guestValue.HasValue && !_petValue.HasValue);

                string label = useAverageForPrice ? 
                    await PricingAvailabilityService.GetPrecalculatedAverageCostOfStayLabelAsync(listing.AveragePrice, fromCurrency, toCurrency, ResourcesShared.InquireOnly) :
                    await PricingAvailabilityService.GetTotalCostOfStayLabelAsync(listing, fromCurrency, toCurrency, _availabilityValue, ResourcesShared.InquireOnly, _guestValue.AdultCount, _petValue.PetCount);

                _priceLabelDictionary.Add(listing.Id, label);
            }
        }
        
        private async Task UpdateFiltersAsync(string uri)
        {
            var querySegments = QueryHelpers.ParseQuery(new Uri(uri).Query);
            foreach (KeyValuePair<string, StringValues> segment in querySegments)
            {           
                _filters.TryGetValue(segment.Key, out var filter);
                if (filter == null) continue;

                AddUniqueFilterAsync(filter.Type, segment);
            }

            _appliedFilters.Values.Where(x => !querySegments.ContainsKey(x.Type.ToString().ToSnakeCase()))
                .ToList()
                .ForEach(x => _appliedFilters.Remove((int)x.Type));

            _skip = 0;
            _top = _skipIncrement;
            await UpdateDataAsync(true);
        }

        private async Task UpdateMapAsync() => await _debouncer.Debounce(async () =>
        {
            await MapBoxService.ClearMarkersAsync();
            _mapBoxData.Clear();

            if (_map == null) return;

            foreach (var listing in _filteredListings)
            {
                if (listing.Longitude == null || listing.Latitude == null || listing.CurrencyCode == null) continue;
                if (!_priceLabelDictionary.ContainsKey(listing.Id)) continue;

                _mapBoxData.Add(await MapBoxService.ConvertSingleListingAsync(listing.Id.ToString(), listing.Longitude.Value, listing.Latitude.Value, GeometryType.Point, _priceLabelDictionary[listing.Id]));
            }

            await _map.UpdateMapAsync(_mapBoxData, _locationValue.Name);
        });

        private string SortQuery() => _appliedFilters.TryGetValue((int)FilterType.SortBy, out IFilterable? sortingFilter) ? SortingService.GetSortingQuery((SortingOptions)sortingFilter) : "";

        private async Task<string> FilterQuery()
        {
            List<string> filterList = new();
                       
            if (_appliedFilters.TryGetValue((int)FilterType.Guests, out IFilterable? guestFilter))
                filterList.Add(GuestFilterService.GetFilterQuery((GuestFilterModel)guestFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Pets, out IFilterable? petFilter))
                filterList.Add(PetFilterService.GetFilterQuery((PetFilterModel)petFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Price, out IFilterable? priceFilter))
                filterList.Add(await PriceFilterService.GetFilterQueryAsync((PriceFilterModel)priceFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Availability, out IFilterable? availabilityFilter))
                filterList.Add(AvailabilityFilterService.GetFilterQuery((AvailabilityFilterModel)availabilityFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Location, out IFilterable? locationFilter))
                filterList.Add(LocationFilterService.GetFilterQuery((LocationFilterModel)locationFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Amenities, out IFilterable? amenitiesFilter))
                filterList.Add(AmenitiesFilterService.GetFilterQuery((AmenitiesFilterModel)amenitiesFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Bedroom, out IFilterable? bedroomFilter))
                filterList.Add(BedroomFilterService.GetFilterQuery((BedroomFilterModel)bedroomFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Bathroom, out IFilterable? bathroomFilter))
                filterList.Add(BathroomFilterService.GetFilterQuery((BathroomFilterModel)bathroomFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.Beds, out IFilterable? bedFilter))
                filterList.Add(BedFilterService.GetFilterQuery((BedFilterModel)bedFilter));
            if (_appliedFilters.TryGetValue((int)FilterType.PropertyType, out IFilterable? propertyTypeFilter))
                filterList.Add(PropertyTypeFilterService.GetFilterQuery((PropertyTypeFilterModel)propertyTypeFilter));
            return string.Join(" AND ", filterList);
        }

        private async Task NextPageAsync()
        {
            if (_listingCount <= 0) return;
            _skip += _top;
            _currentPage++;
            if ((_skip + _top) > _listingCount)
            {
                _top = _listingCount - _skip;
            }
            await UpdateDataAsync();
        }

        private async Task PrevPageAsync()
        {
            if (_listingCount <= 0) return;
            _currentPage--;
            _skip = _skip < 0 ? 0 : _skip - _skipIncrement;
            _top = _skipIncrement;
            await UpdateDataAsync();
        }

        private async Task SetPageAsync(int page)
        {
            if (_listingCount <= 0 || page == _currentPage) return;
            UpdatePagingParameters(page);
            await UpdateDataAsync();
        }

        private void UpdatePagingParameters(int page)
        {
            _currentPage = page;
            _skip = (page * _skipIncrement) - _skipIncrement;
            _top = (_skip + _top) > _listingCount ? _listingCount - _skip : _skipIncrement;
        }

        private void AddUniqueFilterAsync(FilterType type, KeyValuePair<string, StringValues> query)
        {
            _appliedFilters[(int)type] = (type) switch
            {
                FilterType.Location => LocationFilterService.Parse(query),
                FilterType.Availability => AvailabilityFilterService.Parse(query),
                FilterType.Guests => GuestFilterService.Parse(query),
                FilterType.Pets => PetFilterService.Parse(query),
                FilterType.Price => PriceFilterService.Parse(query),
                FilterType.SortBy => SortingService.Parse(query),
                FilterType.Amenities => AmenitiesFilterService.Parse(query),
                FilterType.Bedroom => BedroomFilterService.Parse(query),
                FilterType.Bathroom => BathroomFilterService.Parse(query),
                FilterType.Beds => BedFilterService.Parse(query),
                FilterType.PropertyType => PropertyTypeFilterService.Parse(query),
                _ => throw new NotImplementedException(),
            };
        }

        private void ClearFilter(IFilterable? filter)
        {
            if(filter == null) return;
            _appliedFilters.Remove((int)filter.Type);
            string filterName = filter.Type.ToString().ToSnakeCase();
            string uri = "";
            // Due to complexity of guests filter, we need to explicitly handle it differently here
            if(filter.Type == FilterType.Guests)
            {
                Dictionary<string, object?> parameters = new()
                {
                    { nameof(FilterType.Guests).ToLower(), null },
                    { nameof(FilterType.Adults).ToLower(), null },
                    { nameof(FilterType.Children).ToLower(), null }
                };

                uri = NavigationManager.GetUriWithQueryParameters(parameters);
            }
            else
            {
                uri = NavigationManager.GetUriWithQueryParameter(filterName, null as bool?);
            }
            NavigationManager.NavigateToDecodedUri(uri);
            InvokeAsync(async () => await UpdateDataAsync());
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => InvokeAsync(async () => 
        {
            if(!string.Equals(e.Location, _currentUrl))
            {
                _currentUrl = e.Location;
                await UpdateFiltersAsync(e.Location);
            }
        });

        private void OnMapViewChanged(BoundingBox boundingBox) => InvokeAsync(async () => await _debouncer.Debounce(() => Task.Factory.StartNew(() => NavigationManager.AddQueryParameter(boundingBox))));

        private void OnCurrencyChanged(object? sender, CurrencyChangedEventArgs e) => InvokeAsync(async () => {
            if(_filteredListings != null && _filteredListings.Any())
            {                
                await UpdatePricingLabelsAsync(_filteredListings);
            }
            await UpdateMapAsync();
            StateHasChanged();
        });

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NavigationManager.LocationChanged -= OnLocationChanged;
                    CurrencyService.CurrencyChanged -= OnCurrencyChanged;
                }

                _disposedValue = true;
                if (Configuration.WhiteLabelSettings.AccommodationsPageShowMap)
                {
                    InvokeAsync(async () => await MapBoxService.ClearMarkersAsync());
                }
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
