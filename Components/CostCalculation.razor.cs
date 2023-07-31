using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.PartnerApi;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels.Contracts;
using Microsoft.AspNetCore.Components;
using Stripe;
using System.Diagnostics.CodeAnalysis;

namespace Leavetown.Client.Components
{
    public partial class CostCalculation : IDisposable
    {
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] private IExchangeRateService ExchangeRateService { get; set; } = default!;

        [Parameter, EditorRequired] public int StayNights { get; set; } = default!;
        [Parameter, EditorRequired, DisallowNull]
        public QuoteModel? Quote
        {
            get => _quote; 
            set
            {
                InvokeAsync(async () =>
                {
                    _quote = await ExchangeRateService.ApplyExchangeRateToQuoteAsync(value);
                    StateHasChanged();
                });
            }
        }
        [Parameter] public EventCallback<decimal> CalculationChanged { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private QuoteModel? _quote;
        private bool _disposedValue;
        private CurrencyModel? _currency;

        protected override async Task OnInitializedAsync()
        {
            CurrencyService.CurrencyChanged += OnCurrencyChanged;

            _currency = await CurrencyService.GetLocalCurrencyAsync();
            if (_quote != null) _quote.Currency = CurrencyService.GetCurrencyFromCode(_quote.CurrencyCode) ?? new();
        }

        private void OnCurrencyChanged(object? sender, CurrencyChangedEventArgs e) => 
            InvokeAsync(async () =>
            {
                _currency = e.CurrencyModel;

                if (Quote == null) return;

                _quote = await ExchangeRateService.ApplyExchangeRateToQuoteAsync(Quote);
                StateHasChanged();
            });

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
