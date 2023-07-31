using Leavetown.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Client.Models;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Leavetown.Client.Services.Contracts;
using Radzen;
using Leavetown.Shared.Models;
using Leavetown.Client.Clients.Contracts;
using System.Linq;

namespace Leavetown.Client.Components.Filters
{
    public partial class AdditionalFilters : IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private DialogService DialogService { get; set; } = default!;

        [Inject] private IBedroomFilterService BedroomFilterService { get; set; } = default!;
        [Inject] private IBathroomFilterService BathroomFilterService { get; set; } = default!;
        [Inject] private IAmenitiesFilterService AmenitiesFilterService { get; set; } = default!;
        [Inject] private IPropertyTypeFilterService PropertyTypeFilterService { get; set; } = default!;
        [Inject] private IBedFilterService BedsFilterService { get; set; } = default!;

        [Inject] private IAmenityService AmenityService { get; set; } = default!;
        [Inject] private IPropertyTypeService PropertyTypeService { get; set; } = default!;

        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        [Parameter]
        public BedroomFilterModel BedroomValue
        {
            get => _bedroomFilterModel;
            set
            {
                _bedroomFilterModel = value;
                SetCounters();
            }
        }

        [Parameter]
        public BedFilterModel BedsValue
        {
            get => _bedsFilterModel;
            set
            {
                _bedsFilterModel = value;
                SetCounters();
            }
        }

        [Parameter]
        public BathroomFilterModel BathroomValue
        {
            get => _bathroomFilterModel;
            set
            {
                _bathroomFilterModel = value;
                SetCounters();
            }
        }

        [Parameter] public AmenitiesFilterModel AmenitiesValue
        {
            get => _amenitiesFilterModel;
            set
            {
                _amenitiesFilterModel = value;
                if (_amenitiesFilterModel.Amenities != null)
                {
                    foreach (var amenity in _amenities)
                    {
                        amenity.isActive = _amenitiesFilterModel.Amenities
                            .Any(x => string.Equals(amenity.Value.Name, x.Name, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
                StateHasChanged();
            }
        }

        [Parameter] public PropertyTypeFilterModel PropertyTypeValue
        {
            get => _propertyTypeFilterModel;
            set
            {
                _propertyTypeFilterModel = value;
                if (_propertyTypeFilterModel.PropertyTypes != null)
                {
                    foreach (var propertyType in _propertyTypes)
                    {
                        propertyType.isActive = _propertyTypeFilterModel.PropertyTypes
                            .Any(x => string.Equals(propertyType.Value.Name, x.Name, StringComparison.InvariantCultureIgnoreCase));
                    }
                }
                StateHasChanged();
            }
        }

        private AmenitiesFilterModel _amenitiesFilterModel = new();
        private BedroomFilterModel _bedroomFilterModel = new();
        private BathroomFilterModel _bathroomFilterModel = new();
        private BedFilterModel _bedsFilterModel = new();
        private PropertyTypeFilterModel _propertyTypeFilterModel = new();   

        private Popover _additionalFiltersPopover = new();
        private Counter? _bedroomsCounter = new();
        private Counter? _bedsCounter = new();
        private Counter? _bathroomCounter = new();
        private Dictionary<string, bool> _filterExpandedDictionary = new();
        private bool _disposedValue;
        private List<AdditionalFiltersOption<AmenityModel>> _amenities = new();
        private List<IGrouping<string, AdditionalFiltersOption<AmenityModel>>> _amenityCategories = new();
        private List<AdditionalFiltersOption<PropertyTypeModel>> _propertyTypes = new();

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;

            _propertyTypes = (await PropertyTypeService.GetPropertyTypesAsync())?
                .Select(x => new AdditionalFiltersOption<PropertyTypeModel>(x))
                .Where(x => x != null)
                .ToList() ?? new();
            _amenities = (await AmenityService.GetAmenitiesAsync())?
                .Select(x => new AdditionalFiltersOption<AmenityModel>(x))
                .Where(x => x != null)
                .ToList() ?? new();
            _amenityCategories = _amenities.GroupBy(x => x.Value.AmenityCategoryName).ToList();
            _amenityCategories.ForEach(x => _filterExpandedDictionary.Add(x.Key.ToLower(), false));

            UpdateFromUrl(NavigationManager.Uri);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender) SetCounters();
        }

        public void SetCounters()
        {
            _bedroomsCounter?.Set(_bedroomFilterModel.BedroomCount);
            _bathroomCounter?.Set(_bathroomFilterModel.BathroomCount);
            _bedsCounter?.Set(_bedsFilterModel.BedCount);

            StateHasChanged();
        }

        private void ToggleFirstAmenityFilterExpanded(string filterName)
        {
            _filterExpandedDictionary[filterName] = !_filterExpandedDictionary[filterName];
            StateHasChanged();
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => UpdateFromUrl(e.Location);

        private void UpdateFromUrl(string uri)
        {
            if (uri == null) return;

            Dictionary<string, StringValues>? queryValues = QueryHelpers.ParseQuery(new Uri(uri).Query);
            queryValues.TryGetValue(nameof(FilterType.Bedroom).ToLower(), out StringValues bedroomValue);
            queryValues.TryGetValue(nameof(FilterType.Beds).ToLower(), out StringValues bedsValue);
            queryValues.TryGetValue(nameof(FilterType.Bathroom).ToLower(), out StringValues bathroomValue);
            queryValues.TryGetValue(nameof(FilterType.Amenities).ToLower(), out StringValues amenitiesValue);
            queryValues.TryGetValue(nameof(FilterType.PropertyType).ToSnakeCase(), out StringValues proprtyTypeValue);

            BedroomValue = BedroomFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Bedroom).ToLower(), bedroomValue));
            BedsValue = BedsFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Beds).ToLower(), bedsValue));
            BathroomValue = BathroomFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Bathroom).ToLower(), bathroomValue));
            AmenitiesValue = AmenitiesFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Amenities).ToLower(), amenitiesValue));
            PropertyTypeValue = PropertyTypeFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.PropertyType).ToSnakeCase(), proprtyTypeValue));
        }

        private void SaveFilters ()
        {
            var additionalFilterModel = new AdditionalFiltersModel()
            {
                AmenitiesFilterModel = _amenitiesFilterModel,
                PropertyTypeFilterModel = _propertyTypeFilterModel,
                BathroomFilterModel = _bathroomFilterModel,
                BedroomFilterModel = _bedroomFilterModel,
                BedsFilterModel = _bedsFilterModel
            };

            NavigationManager.AddQueryParameter(additionalFilterModel);
            DialogService.Close(true);
        }

        private void OnAmenitiesFilterValueChange()
        {
            _amenitiesFilterModel = new AmenitiesFilterModel()
            {
                Amenities = _amenities
                    .Where(x => x.isActive)
                    .Select(x => x.Value)
                    .ToList()
            };
        }

        private void OnPropertyTypeFilterValueChange()
        {
            _propertyTypeFilterModel = new PropertyTypeFilterModel()
            {
                PropertyTypes = _propertyTypes
                    .Where(x => x.isActive)
                    .Select(x => x.Value)
                    .ToList()
            };
        }

        private void OnBedroomFilterValueChange()
        {
            _bedroomFilterModel = new BedroomFilterModel() { BedroomCount = _bedroomsCounter?.Value ?? 0 };
        }

        private void OnBedsFilterValueChange()
        {
            _bedsFilterModel = new BedFilterModel() { BedCount = _bedsCounter?.Value ?? 0 };
        }

        private void OnBathroomFilterValueChange()
        {
            _bathroomFilterModel = new BathroomFilterModel() { BathroomCount = _bathroomCounter?.Value ?? 0 };
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
