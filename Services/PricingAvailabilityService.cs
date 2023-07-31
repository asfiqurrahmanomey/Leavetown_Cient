using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Settings;
using Leavetown.Shared.Helpers;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels.Contracts;

namespace Leavetown.Client.Services
{
    public class PricingAvailabilityService : IPricingAvailabilityService
    {
        private IExchangeRateService _exchangeRateService;
        private IQuoteService _quoteService;
        private Configuration _configuration;

        public PricingAvailabilityService(IExchangeRateService exchangeRateService, IQuoteService quoteService, Configuration configuration)
        {
            _exchangeRateService = exchangeRateService;
            _quoteService = quoteService;
            _configuration = configuration;
        }

        public bool IsInquireOnly(IEnumerable<PricingAvailabilityModel>? pricingAvailabilities, decimal? costOfStay) =>
            (pricingAvailabilities?.All(x => x != null && !x.IsAvailable) ?? false) || costOfStay == null || costOfStay == 0M;

        public async Task<string> GetPrecalculatedAverageCostOfStayLabelAsync(
            decimal averagePrice,
            CurrencyModel fromCurrency,
            CurrencyModel toCurrency,
            string inquireOnlyString)
        {
            ExchangeRateConversionModel exchangeRateConversion = await _exchangeRateService.GetExchangeRateConversionModelAsync(fromCurrency, toCurrency);
            decimal? costOfStay = _exchangeRateService.CalculateExchangeRate(averagePrice, exchangeRateConversion);
            string? label = costOfStay == null || costOfStay == 0M ? inquireOnlyString : $"{toCurrency?.Symbol}{costOfStay:##,#}";

            return label;
        }

        public async Task<string> GetAverageCostOfStayLabelAsync(
            IEnumerable<PricingAvailabilityModel>? pricingAvailabilities,
            CurrencyModel fromCurrency,
            CurrencyModel toCurrency,
            AvailabilityFilterModel? availability,
            string inquireOnlyString)
        {
            decimal? costOfStay = await CalculateAverageCostOfStayAsync(pricingAvailabilities, toCurrency, fromCurrency!, availability);
            string? label = IsInquireOnly(pricingAvailabilities, costOfStay) ? inquireOnlyString : $"{toCurrency?.Symbol}{costOfStay:##,#}";

            return label;
        }

        public async Task<string> GetTotalCostOfStayLabelAsync(
            ListingViewModel listing,
            CurrencyModel fromCurrency,
            CurrencyModel toCurrency,
            AvailabilityFilterModel? availability,
            string inquireOnlyString,
            int adults,
            int pets)
        {
            decimal? costOfStay = await CalculateTotalCostOfStayAsync(listing, toCurrency, fromCurrency!, availability, adults, pets);
            string? label = IsInquireOnly(listing.PricingAvailabilities, costOfStay) ? inquireOnlyString : $"{toCurrency?.Symbol}{costOfStay:##,#}";

            return label;
        }

        public IEnumerable<PricingAvailabilityModel>? GetPricingAvailabilitiesBetweenDates<T>(T listing, AvailabilityFilterModel? availability) where T : IListingViewModel =>
            availability != null && availability.HasValue ?
                listing.PricingAvailabilities?.Where(x => x != null && x.Date >= availability.Start && x.Date < availability.End && x.IsAvailable).Cast<PricingAvailabilityModel>() :
                listing.PricingAvailabilities?.Where(x => x != null && x.IsAvailable).Cast<PricingAvailabilityModel>();

        public IEnumerable<PricingAvailabilityModel>? GetPricingAvailabilitiesBetweenDates(IEnumerable<PricingAvailabilityModel> pricingAvailabilities, AvailabilityFilterModel? availability) =>
            availability != null && availability.HasValue ?
                pricingAvailabilities?.Where(x => x != null && x.Date >= availability.Start && x.Date < availability.End && x.IsAvailable) :
                pricingAvailabilities?.Where(x => x != null && x.IsAvailable);

        public async Task<decimal?> CalculateAverageCostOfStayAsync(IEnumerable<PricingAvailabilityModel>? pricingAvailabilities, CurrencyModel toCurrency, CurrencyModel fromCurrency, AvailabilityFilterModel? availability)
        {
            return await CalculateCostOfStayAsync(pricingAvailabilities, toCurrency, fromCurrency, availability, x => x.Average());
        }

        public async Task<decimal?> CalculateTotalCostOfStayAsync(IListingViewModel listing, CurrencyModel toCurrency, CurrencyModel fromCurrency, AvailabilityFilterModel? availability, int adults, int pets)
        {
            return await CalculateCostOfStayAsync(listing.PricingAvailabilities, toCurrency, fromCurrency, availability, x =>
            {
                var price = x.Sum();

                if (availability != null && availability.HasValue && _configuration.CurrencyCodesToShowQuoteTotal.Contains(listing.CurrencyCode))
                    price = _quoteService.CalculateQuoteTotal(price, listing, availability.Start.StayNights(availability.End), adults, pets);

                return price;
            });
        }

        private async Task<decimal?> CalculateCostOfStayAsync(
            IEnumerable<PricingAvailabilityModel>? pricingAvailabilities,
            CurrencyModel toCurrency,
            CurrencyModel fromCurrency,
            AvailabilityFilterModel? availability, 
            Func<List<decimal>, decimal> calculatePrice)
        {
            if (pricingAvailabilities == null) return null;

            ExchangeRateConversionModel exchangeRateConversion = await _exchangeRateService.GetExchangeRateConversionModelAsync(fromCurrency, toCurrency);

            List<decimal>? prices = GetPricingAvailabilitiesBetweenDates(pricingAvailabilities, availability)?.Select(x => x.Price).ToList();

            if (prices == null || !prices.Any()) return null;

            decimal price = calculatePrice(prices);

            return _exchangeRateService.CalculateExchangeRate(price, exchangeRateConversion);
        }
    }
}
