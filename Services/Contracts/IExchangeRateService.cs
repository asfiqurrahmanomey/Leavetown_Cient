using Leavetown.Client.Models;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IExchangeRateService
    {
        Task<decimal?> ConvertToCurrencyAsync(decimal price, string currencyCode);
        decimal CalculateExchangeRate(decimal price, ExchangeRateConversionModel exchangeRateConversion);
        decimal CalculateExchangeRate(decimal price, decimal fromRate, decimal toRate);
        Task<QuoteModel> ApplyExchangeRateToQuoteAsync(QuoteModel quote);
        Task<decimal> GetRateFromCurrencyAsync(CurrencyModel currency, Dictionary<string, decimal>? rates = null);
        Task<ExchangeRateConversionModel> GetExchangeRateConversionModelAsync(CurrencyModel fromCurrency, CurrencyModel toCurrency);
        Task<Dictionary<string, decimal>?> GetCurrentRatesAsync();
    }
}
