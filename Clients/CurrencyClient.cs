using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Clients
{
    public class CurrencyClient : ICurrencyClient
    {
        private IOpenSearchClient _openSearchClient;
        private ILeavetownClient _leavetownClient;

        public CurrencyClient(IOpenSearchClient openSearchClient, ILeavetownClient leavetownClient)
        {
            _openSearchClient = openSearchClient;
            _leavetownClient = leavetownClient;
        }

        public async Task<IEnumerable<CurrencyModel>?> GetCurrencyModelsAsync() => 
            await _openSearchClient.GetDataAsync<CurrencyModel>("SELECT * FROM currencies");

        public async Task<Dictionary<string, decimal>?> GetExchangeRatesAsync() => await _leavetownClient.GetExchangeRatesAsync();
    }
}
