using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Components;
using Leavetown.Client.Constants;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class CurrencyDropDown : IDisposable
    {
        [Inject] private ICurrencyService CurrencyService { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;

        [Parameter] public string Id { get; set; } = "";        
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;

        private List<CurrencyModel> _currencies = new();
        private CurrencyModel? _selectedCurrency;

        private Popover? _popover;
        private bool _disposedValue;

        protected override async Task OnInitializedAsync()
        {
            _currencies = CurrencyService.Currencies ?? new();
            if (_currencies.Any())
            {
                await LoadInitialCurrencyAsync();
            }

            CurrencyService.CurrencyChanged += OnCurrencyChanged;
            LocalStorageService.LocalStorageChanged += OnLocalStorageChanged;
        }

        private void OnLocalStorageChanged(object? sender, LocalStorageChangedEventArgs e)
        {
            if(_selectedCurrency == null)
            {
                if (e.Key != "currency") return;

                _currencies = CurrencyService.Currencies ?? new List<CurrencyModel>();
                if (!_currencies.Any()) return;

                InvokeAsync(async () =>
                {
                    await LoadInitialCurrencyAsync();
                    StateHasChanged();
                });
            }
        }

        private async Task LoadInitialCurrencyAsync()
        {
            CurrencyModel? currency = await CurrencyService.GetLocalCurrencyAsync();

            SelectCurrency(currency, false);
        }
        
        private void OnCurrencyChanged(object? sender, Models.Events.CurrencyChangedEventArgs e)
        {
            _selectedCurrency = e.CurrencyModel;
            StateHasChanged();
        }

        private void SelectCurrency(CurrencyModel? currency, bool toggleCardVisibility = true)
        {
            if (toggleCardVisibility) _popover?.ToggleCardVisibility();
            if (currency == null || currency.Code == _selectedCurrency?.Code) return;

            _selectedCurrency = currency;
            InvokeAsync(async () => await CurrencyService.SetLocalCurrencyAsync(currency));            
            StateHasChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CurrencyService.CurrencyChanged -= OnCurrencyChanged;
                    LocalStorageService.LocalStorageChanged -= OnLocalStorageChanged;
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
