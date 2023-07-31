using Leavetown.Client.Models.Events;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface ICurrencyService
    {
        List<CurrencyModel> Currencies { get; }
        event EventHandler<CurrencyChangedEventArgs> CurrencyChanged;
        Task SetLocalCurrencyAsync(CurrencyModel? currency = null);
        Task<CurrencyModel?> GetLocalCurrencyAsync();
        CurrencyModel? GetCurrencyFromCode(string currencyCode);        
    }
}
