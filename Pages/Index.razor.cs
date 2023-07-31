using Leavetown.Client.Clients;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IListingClient ListingClient { get; set; } = default!;
        [Inject] private IPricingAvailabilityClient PricingAvailabilityClient { get; set; } = default!;
        [Inject] private IPricingAvailabilityService PricingAvailabilityService { get; set; } = default!;
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;

        [Parameter] public string? Culture { get; set; }
        [CascadingParameter] public Configuration Configuration { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;


        private List<ListingViewModel> _featuredProperties = new();
        private Dictionary<int, string> _priceLabelDictionary = new();

        private void OnCurrencyChanged(object? sender, CurrencyChangedEventArgs e) => InvokeAsync(async () => {
            if (_featuredProperties != null && _featuredProperties.Any())
            {                
                await UpdatePricingLabelsAsync(_featuredProperties);
            }
            StateHasChanged();
        });

        protected override async Task OnInitializedAsync()
        {
            CurrencyService.CurrencyChanged += OnCurrencyChanged;
                      
            _featuredProperties = (await ListingClient.GetListingModelsAsync(featuredOnly: true) ??
                         Enumerable.Empty<ListingViewModel>()).ToList();
            
            await UpdatePricingLabelsAsync(_featuredProperties);
        }

        private void OnSearchClick()
        {
            string query = new Uri(NavigationManager.Uri).Query;
            NavigationManager.NavigateTo($"{Culture}/accommodations/{query}");
        }

        private async Task UpdatePricingLabelsAsync(IEnumerable<ListingViewModel>? featuredPropertyPrices)
        {
            _priceLabelDictionary.Clear();

            if (featuredPropertyPrices == null) return;

            var toCurrency = await CurrencyService.GetLocalCurrencyAsync();

            if (toCurrency == null) return;

            foreach (var featuredProperty in featuredPropertyPrices.Where(featuredProperty => featuredProperty.CurrencyCode != null))
            {
                if (featuredProperty.CurrencyCode == null) continue;

                CurrencyModel? fromCurrency = CurrencyService.GetCurrencyFromCode(featuredProperty.CurrencyCode);
                if (fromCurrency == null) continue;
                
                string label = await PricingAvailabilityService.GetPrecalculatedAverageCostOfStayLabelAsync(featuredProperty.AveragePrice, fromCurrency, toCurrency, ResourcesShared.InquireOnly);

                if(_priceLabelDictionary.ContainsKey(featuredProperty.Id)) _priceLabelDictionary[featuredProperty.Id] = label;
                else _priceLabelDictionary.Add(featuredProperty.Id, label);
            }
        }

        public override void Dispose()
        {
            CurrencyService.CurrencyChanged -= OnCurrencyChanged;
            base.Dispose();
        }
    }
}