using Leavetown.Client.Models.Events;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services
{
    public class CurrencyService : ICurrencyService
    {
        private ILocalStorageService _localStorageService;        
        private Configuration _configuration;

        private List<CurrencyModel> _currencies = new List<CurrencyModel>
        {
            new CurrencyModel{Name = "Canadian Dollar", Code = "CAD", Symbol = "CA$"},
            new CurrencyModel{Name = "Euro", Code = "EUR", Symbol = "€"},
            new CurrencyModel{Name = "British Pound", Code = "GBP", Symbol = "£"},
            new CurrencyModel{Name = "US Dollar", Code = "USD", Symbol = "US$"}
        };

        public event EventHandler<CurrencyChangedEventArgs> CurrencyChanged = default!;
        public List<CurrencyModel> Currencies { get => _currencies; }

        public CurrencyService(
            ILocalStorageService localStorageService,            
            Configuration configuration)
        {
            _localStorageService = localStorageService;            
            _configuration = configuration;
        }

        public async Task SetLocalCurrencyAsync(CurrencyModel? currency = null)
        {
            bool shouldPersist;
            if (currency == null)
            {
                Tuple<CurrencyModel, bool> tuple = await GetLocalCurrencyOrDefaultAsync();
                currency = tuple.Item1;
                shouldPersist = tuple.Item2;
            }
            else
            {
                shouldPersist = true;
            }

            if (shouldPersist)
            {
                await _localStorageService.SetStorageValueAsync("currency", currency);
            }
            
            if (CurrencyChanged != null) CurrencyChanged.Invoke(this, new CurrencyChangedEventArgs(currency));
        }

        public async Task<CurrencyModel?> GetLocalCurrencyAsync() => (await GetLocalCurrencyOrDefaultAsync()).Item1;

        private async Task<Tuple<CurrencyModel, bool>> GetLocalCurrencyOrDefaultAsync()
        {
            CurrencyModel? currency = await _localStorageService.GetStorageValueAsync<CurrencyModel>("currency");

            currency ??= _currencies.FirstOrDefault(currency => currency.Code == _configuration?.WhiteLabelSettings?.DefaultCurrencyCode);
            
            bool shouldPersist;
            if (currency == null)
            {
                currency = _currencies.First();
                shouldPersist = false;
            }
            else
            {
                shouldPersist = true;
            }
            
            return new Tuple<CurrencyModel, bool>(currency, shouldPersist);
        }

        public CurrencyModel? GetCurrencyFromCode(string currencyCode)
        {
            if (string.IsNullOrEmpty(currencyCode)) return null;
            return _currencies.Where(x => string.Equals(x.Code, currencyCode, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
        }
    }
}
