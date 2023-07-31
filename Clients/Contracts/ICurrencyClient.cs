using Leavetown.Shared.Models;
using System.Globalization;

namespace Leavetown.Client.Clients.Contracts
{
    public interface ICurrencyClient
    {
        Task<IEnumerable<CurrencyModel>?> GetCurrencyModelsAsync();
        Task<Dictionary<string, decimal>?> GetExchangeRatesAsync();
    }
}
