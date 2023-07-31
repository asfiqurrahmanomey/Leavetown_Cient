using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Helpers;
using Leavetown.Shared.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Leavetown.Client.Services;

public class ExchangeRateService : IExchangeRateService
{
    public const int RoundingDecimalPlaces = 5;

    private ICurrencyService _currencyService;
    private ILogger<ExchangeRateService> _logger;
    private ICurrencyClient _currencyClient;
    private ILocalStorageService _localStorageService;
    private const string _baseCurrencyCode = "CAD";
    private bool _ratesLoaded = false;

    public ExchangeRateService(
        ICurrencyService currencyService,
        ILogger<ExchangeRateService> logger,
        ICurrencyClient currencyClient,
        ILocalStorageService localStorageService)
    {
        _currencyService = currencyService;
        _logger = logger;
        _currencyClient = currencyClient;
        _localStorageService = localStorageService;
    }

    public bool RatesLoaded { get => _ratesLoaded; }

    public async Task<decimal?> ConvertToCurrencyAsync(decimal price, string currencyCode )
    {

        CurrencyModel? fromCurrency = await _currencyService.GetLocalCurrencyAsync();
        if (fromCurrency == null) return null;

        CurrencyModel? toCurrency = _currencyService.GetCurrencyFromCode(currencyCode);
        if (toCurrency == null) return null;

        decimal fromRate = await GetRateFromCurrencyAsync(fromCurrency);
        decimal toRate = await GetRateFromCurrencyAsync(toCurrency);

        return CalculateExchangeRate(price, fromRate, toRate);
    }

    public decimal CalculateExchangeRate(decimal price, ExchangeRateConversionModel exchangeRateConversion) =>
        CalculateExchangeRate(price, exchangeRateConversion.From, exchangeRateConversion.To);

    public decimal CalculateExchangeRate(decimal price, decimal fromRate, decimal toRate)
    {
        //TODO: Investigate if we should just return 0 or if inputting a 0 is something we should throw an error for.
        if (price == 0 || fromRate == 0 || toRate == 0) return 0;
        if (fromRate == toRate) return price;

        decimal fromValue = decimal.Multiply(price, fromRate);
        decimal toValue = decimal.Multiply(price, toRate);
        decimal trueRate = decimal.Divide(fromValue, toValue);

        decimal result = decimal.Round(decimal.Divide(price, trueRate), RoundingDecimalPlaces, MidpointRounding.AwayFromZero);
        return result;
    }

    public async Task<QuoteModel> ApplyExchangeRateToQuoteAsync(QuoteModel quote)
    {
        try
        {
            CurrencyModel? fromCurrency = _currencyService.GetCurrencyFromCode(quote.CurrencyCode);
            CurrencyModel? toCurrency = await _currencyService.GetLocalCurrencyAsync();

            if (toCurrency == null || fromCurrency == null)
            {
                throw new ArgumentNullException("Currency is invalid");
            }

            if (string.Equals(fromCurrency.Code, toCurrency.Code)) return quote;

            Dictionary<string, decimal>? rates = await GetCurrentRatesAsync();
            decimal fromRate = await GetRateFromCurrencyAsync(fromCurrency, rates);
            decimal toRate = await GetRateFromCurrencyAsync(toCurrency, rates);

            QuoteModel quoteCopy = quote.ShallowCopy();

            // If a new fee type is added, they will need to be included here in order to apply the currency exchange calculation
            if (quoteCopy.PetFees != 0M) quoteCopy.PetFees = CalculateExchangeRate(quoteCopy.PetFees, fromRate, toRate);
            if (quoteCopy.ServiceFee != 0M) quoteCopy.ServiceFee = CalculateExchangeRate(quoteCopy.ServiceFee, fromRate, toRate);
            if (quoteCopy.CleaningFee != 0M) quoteCopy.CleaningFee = CalculateExchangeRate(quoteCopy.CleaningFee, fromRate, toRate);
            if (quoteCopy.AutohostFee != 0M) quoteCopy.AutohostFee = CalculateExchangeRate(quoteCopy.AutohostFee, fromRate, toRate);
            if (quoteCopy.PromotionDiscount != 0M) quoteCopy.PromotionDiscount = CalculateExchangeRate(quoteCopy.PromotionDiscount, fromRate, toRate);
            if (quoteCopy.ExtraAdultFees != 0M) quoteCopy.ExtraAdultFees = CalculateExchangeRate(quoteCopy.ExtraAdultFees, fromRate, toRate);

            quoteCopy.RentalTotal = CalculateExchangeRate(quoteCopy.RentalTotal, fromRate, toRate);
            quoteCopy.BalanceAmount = CalculateExchangeRate(quoteCopy.BalanceAmount, fromRate, toRate);
            quoteCopy.DepositAmount = CalculateExchangeRate(quoteCopy.DepositAmount, fromRate, toRate);
            quoteCopy.TaxesAndFees = CalculateExchangeRate(quoteCopy.TaxesAndFees, fromRate, toRate);
            quoteCopy.PreTaxTotal = CalculateExchangeRate(quoteCopy.PreTaxTotal, fromRate, toRate);
            quoteCopy.OriginalRentalTotal = CalculateExchangeRate(quoteCopy.OriginalRentalTotal, fromRate, toRate);
            quoteCopy.Total = CalculateExchangeRate(quoteCopy.Total, fromRate, toRate);

            quoteCopy.Currency = toCurrency;
            quoteCopy.CurrencyCode = toCurrency.Code; 
            
            quoteCopy.PrePromotionalTotal = CalculateExchangeRate(quoteCopy.PrePromotionalTotal, fromRate, toRate);

            return quoteCopy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: ex.Message);
            throw;
        }
    }

    public async Task<decimal> GetRateFromCurrencyAsync(CurrencyModel currency, Dictionary<string, decimal>? rates = null)
    {
        decimal? rate = (rates ?? await GetCurrentRatesAsync())?
            .Where(x => x.Key == currency.Code)
            .Select(x => x.Value)
            .SingleOrDefault();

        if (rate == null || rate == 0) throw new ArgumentNullException($"Could not retrieve rate from currency code: {currency.Code}. Number of rates retrived: {rates?.Count() ?? 0}.");

        return rate.Value;
    }

    public async Task<ExchangeRateConversionModel> GetExchangeRateConversionModelAsync(CurrencyModel fromCurrency, CurrencyModel toCurrency)
    {
        Dictionary<string, decimal>? rates = await GetCurrentRatesAsync();

        decimal fromRate = await GetRateFromCurrencyAsync(fromCurrency, rates);
        decimal toRate = await GetRateFromCurrencyAsync(toCurrency, rates);

        return new ExchangeRateConversionModel(toRate, fromRate);
    }

    private async Task<Dictionary<string, decimal>?> SetCurrentRatesAsync()
    {
        Dictionary<string, decimal>? rates = await _currencyClient.GetExchangeRatesAsync();
        if (rates == null)
        {
            _logger.LogError("Failed to retrieve rates from API. Using mock data for rates.");
            rates = FallbackRateFactory.Build();
        }
        _ratesLoaded = true;
        await _localStorageService.SetStorageValueAsync("rates", rates);
        return rates;
    }

    public async Task<Dictionary<string, decimal>?> GetCurrentRatesAsync()
    {
        Dictionary<string, decimal>? rates = await _localStorageService.GetStorageValueAsync<Dictionary<string, decimal>>("rates");
        if (rates == null) rates = await SetCurrentRatesAsync();
        return rates;
    }
}
