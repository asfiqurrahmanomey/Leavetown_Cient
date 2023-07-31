using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.PartnerApi;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class BookingPaymentGrid : IDisposable
    {
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] private IExchangeRateService ExchangeRateService { get; set; } = default!;
        [Inject] private IRenderState RenderState { get; set; } = default!;

        [Parameter, EditorRequired] public BookingViewModel Booking { get; set; } = default!;
        [Parameter, EditorRequired] public ListingDetailsViewModel Listing { get; set; } = default!;
        [Parameter, EditorRequired] public QuoteModel Quote { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private QuoteModel _quote = new();
        private bool _disposedValue;
        private CurrencyModel? _currency;

        //TODO: Code is very similar to CostCalculation code behind. Maybe look into a way of consolidating the similar functionality

        protected override async Task OnInitializedAsync()
        {
            CurrencyService.CurrencyChanged += OnCurrencyChanged;
            _currency = await CurrencyService.GetLocalCurrencyAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Quote != null)
            {
                _quote = await ExchangeRateService.ApplyExchangeRateToQuoteAsync(Quote);
                await UpdateQuoteDataAsync();
            }
            await base.OnParametersSetAsync();
        }

        private void OnCurrencyChanged(object? sender, CurrencyChangedEventArgs e) => InvokeAsync(async () =>
            {
                _currency = e.CurrencyModel;
                await UpdateQuoteDataAsync();
                StateHasChanged();
            });

        private async Task UpdateQuoteDataAsync()
        {
            _quote = await ExchangeRateService.ApplyExchangeRateToQuoteAsync(Quote);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
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
}
