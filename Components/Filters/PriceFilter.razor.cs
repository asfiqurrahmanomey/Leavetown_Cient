using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Components.Filters.Contracts;
using Leavetown.Client.Models;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Leavetown.Client.Components.Filters
{
    public partial class PriceFilter : IFilterComponent, IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IExchangeRateService ExchangeRateService { get; set; } = default!;
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] private IPriceFilterService PriceFilterService { get; set; } = default!;
        [Inject] private IPricingAvailabilityClient PricingAvailabilityClient { get; set; } = default!;

        [Parameter] public PriceFilterModel? PriceValue { get; set; }
        [Parameter] public EventCallback<PriceFilterModel> PriceValueChanged { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private Popover? _pricePopover = new();

        private List<ChartData> _chartData = new();
        private IEnumerable<int> _sliderValues = new int[2];
        private PriceFilterModel _previousPriceFilterModel = default!;
        private decimal _priceMin = 0;
        private decimal _priceMax = PriceFilterModel.AbsoluteMaximum;
        private List<decimal>? _prices;
        private CurrencyModel? _currency;
        private bool _disposedValue;
        private Debouncer _debouncer = new();

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            CurrencyService.CurrencyChanged += OnCurrencyChanged;
            await SetCurrencyAsync();
            await SetPricesAsync();
            BuildChartData();
            await UpdateFromUrlAsync(NavigationManager.Uri);
        }
        
        public void Reset(FilterType? filterType = null)
        {
            ResetPriceSlider();
            StateHasChanged();
            OnSliderMouseLeave();
        }

        public void Expand() => _pricePopover?.ToggleCardVisibility();

        private async Task SetCurrencyAsync()
        {
            _currency = await CurrencyService.GetLocalCurrencyAsync();

            if (_currency == null) throw new Exception("Cannot retrieve local currency.");
        }

        private async Task SetPricesAsync()
        {
            _prices = (await PricingAvailabilityClient.GetGroupedPricesFromListingsAsync())?.ToList();
            decimal? rateCAD = (await ExchangeRateService.GetCurrentRatesAsync())?.GetValueOrDefault("CAD");

            if (rateCAD == null) throw new Exception("Cannot retrieve base rate of CAD from exchange rates in memory.");

            decimal localRate = await ExchangeRateService.GetRateFromCurrencyAsync(_currency!);

            _priceMax = (int)ExchangeRateService.CalculateExchangeRate(PriceFilterModel.AbsoluteMaximum, rateCAD.Value, localRate);


            PriceValue = new PriceFilterModel
            {
                Maximum = PriceValue?.Maximum ?? (int)_priceMax,
                Minimum = PriceValue?.Minimum ?? (int)_priceMin,
            };

            _previousPriceFilterModel = new PriceFilterModel
            {
                Maximum = PriceValue?.Maximum ?? (int)_priceMax,
                Minimum = PriceValue?.Minimum ?? (int)_priceMin,
            };

            StateHasChanged();
        }

        private async Task UpdateFromUrlAsync(string uri)
        {
            if (uri == null) return;

            Dictionary<string, StringValues>? queryValues = QueryHelpers.ParseQuery(new Uri(uri).Query);
            queryValues.TryGetValue(nameof(FilterType.Price).ToLower(), out var value);
            PriceFilterModel price = value != StringValues.Empty ?
                PriceFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Price).ToLower(), value)) :
                new PriceFilterModel
                {
                    Minimum = (int)_priceMin,
                    Maximum = (int)_priceMax
                };

            _previousPriceFilterModel = new PriceFilterModel
            {
                Minimum = price.Minimum,
                Maximum = price.Maximum,
            };

            PriceValue = new PriceFilterModel
            {
                Minimum = price.Minimum,
                Maximum = price.Maximum,
            };
            _sliderValues = new int[2] { PriceValue.Minimum, PriceValue.Maximum };

            await PriceValueChanged.InvokeAsync(PriceValue);
            StateHasChanged();
        }

        private void ResetPriceSlider()
        {
            _sliderValues = new int[] { (int)_priceMin, (int)_priceMax };
            PriceValue = new PriceFilterModel() { Minimum = (int)_priceMin, Maximum = (int)_priceMax };
            StateHasChanged();
        }

        private void BuildChartData()
        {
            _chartData.Clear();
            Dictionary<int, int> _items = new();

            int chartIncrement = 50;

            if (_prices == null || !_prices.Any()) return;

            foreach (int price in PriceValue != null && PriceValue.HasValue ? _prices
                .Where(x => x > PriceValue.Minimum && x < PriceValue.Maximum)
                .OrderBy(x => x) : _prices.OrderBy(x => x))
            {

                if (price == 0) continue;

                int key = (price / chartIncrement) * chartIncrement;

                if (_items.ContainsKey(key))
                {
                    _items[key]++;
                }
                else
                {
                    _items.Add(key, 1);
                }
            }

            _items.OrderBy(x => x.Key).ToList()
                .ForEach(x => _chartData.Add(new ChartData { Price = x.Key, NumberOfEntries = x.Value }));

            InvokeAsync(StateHasChanged);
        }

        private int[] GetSliderValues(IEnumerable<int> sliderValues)
        {
            var sliderValueArray = sliderValues.ToArray();

            // HACK: fix for difficulty selecting min and max values of slider
            var sliderOffset = 20;
            if (sliderValueArray[0] <= sliderOffset) sliderValueArray[0] = (int)_priceMin;
            if (sliderValueArray[1] >= _priceMax - sliderOffset) sliderValueArray[1] = (int)_priceMax;

            return sliderValueArray;
        }

        private void OnSliderChanged(IEnumerable<int> args)
        {
            var sliderValues = GetSliderValues(args);

            if (!sliderValues.Any()) return;
            if (sliderValues[0] > sliderValues[1]) throw new Exception($"Slider Minimum value {sliderValues[0]} cannot be greater than Maximum value {sliderValues[1]}");

            PriceValue ??= new();
            if (PriceValue.Minimum == sliderValues[0] && PriceValue.Maximum == sliderValues[1])
                return;

            _previousPriceFilterModel.Minimum = PriceValue.Minimum;
            _previousPriceFilterModel.Maximum = PriceValue.Maximum;
            PriceValue.Minimum = sliderValues[0];
            PriceValue.Maximum = sliderValues[1];
        }

        private void OnSliderMouseLeave() => InvokeAsync(async () =>
        {
            if (PriceValue?.Minimum == _previousPriceFilterModel.Minimum && PriceValue?.Maximum == _previousPriceFilterModel.Maximum)
                return;

            await _debouncer.Debounce(async () => await UpdatePrice());
        });

        private async Task UpdatePrice()
        {
            BuildChartData();
            decimal? rateCAD = (await ExchangeRateService.GetCurrentRatesAsync())?.GetValueOrDefault("CAD");

            if (rateCAD == null) throw new Exception("Cannot retrieve base rate of CAD from exchange rates in memory.");

            decimal localRate = await ExchangeRateService.GetRateFromCurrencyAsync(_currency!);
            // Do not filter when values are at min and max
            if (PriceValue?.Minimum == 0 && PriceValue?.Maximum == (int)ExchangeRateService.CalculateExchangeRate(PriceFilterModel.AbsoluteMaximum, rateCAD.Value, localRate))
            {
                PriceValue = null!;
                NavigationManager.RemoveQueryStringByKey(nameof(FilterType.Price).ToLower());
            }
            else
            {
                NavigationManager.AddQueryParameter(PriceValue);
            }
            await PriceValueChanged.InvokeAsync(PriceValue);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => InvokeAsync(async () => await UpdateFromUrlAsync(e.Location));

        private async void OnCurrencyChanged(object? sender, CurrencyChangedEventArgs e)
        {
            if (string.Equals(_currency?.Code, e.CurrencyModel.Code)) return;

            _currency = e.CurrencyModel;
            await SetPricesAsync();
            Reset();
            OnSliderMouseLeave();
            StateHasChanged();
        }

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
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class ChartData
    {
        public int Price { get; set; }
        public int NumberOfEntries { get; set; }
    }
}
