using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class PriceFilterService : IPriceFilterService
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ICurrencyService _currencyService;

        public PriceFilterService(IExchangeRateService exchangeRateService, ICurrencyService currencyService)
        {
            _exchangeRateService = exchangeRateService;
            _currencyService = currencyService;
        }

        public PriceFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            PriceFilterModel filter = new();
            var prices = query.Value.ToString().Split('~', StringSplitOptions.RemoveEmptyEntries);

            filter.Minimum = Convert.ToInt32(prices[0]);
            filter.Maximum = Convert.ToInt32(prices[1]);

            return filter;
        }

        public async Task<string> GetFilterQueryAsync(PriceFilterModel filter)
        {
           
            if(_currencyService.Currencies == null)
                return string.Empty;

            StringBuilder stringBuilder = new();
            stringBuilder.Append($"( ");
            foreach (var currency in _currencyService.Currencies)
            {
                if(currency.Code != _currencyService.Currencies.First().Code)
                    stringBuilder.Append($" OR ");
                var baseCurrencyMinimum = await _exchangeRateService.ConvertToCurrencyAsync(filter.Minimum, currency.Code);
                var baseCurrencyMaximum = await _exchangeRateService.ConvertToCurrencyAsync(filter.Maximum, currency.Code);
                var baseCurrencyAbsoluteMaximum = await _exchangeRateService.ConvertToCurrencyAsync(PriceFilterModel.AbsoluteMaximum, currency.Code);
                stringBuilder.Append($" (  currency_code ='{currency.Code}' ");
                stringBuilder.Append(" AND");
                stringBuilder.Append($" average_price >= {baseCurrencyMinimum}");
                if (baseCurrencyMaximum != baseCurrencyAbsoluteMaximum)
                {
                    stringBuilder.Append(" AND");
                    stringBuilder.Append($" average_price <= {baseCurrencyMaximum})");
                }
                else
                {
                    stringBuilder.Append(')');
                }
                
            }
            stringBuilder.Append($") ");
            return stringBuilder.ToString();
        }

        string IFilterService<PriceFilterModel>.GetFilterQuery(PriceFilterModel filter)
        {
            throw new NotImplementedException();
        }
    }
}
