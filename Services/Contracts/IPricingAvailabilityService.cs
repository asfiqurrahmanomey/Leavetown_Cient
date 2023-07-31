using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels.Contracts;

namespace Leavetown.Client.Services.Contracts
{
    public interface IPricingAvailabilityService
    {
        bool IsInquireOnly(IEnumerable<PricingAvailabilityModel>? pricingAvailabilities, decimal? costOfStay);
        Task<string> GetPrecalculatedAverageCostOfStayLabelAsync(decimal averagePrice, CurrencyModel fromCurrency, CurrencyModel toCurrency, string inquireOnlyString);
        Task<string> GetAverageCostOfStayLabelAsync(IEnumerable<PricingAvailabilityModel>? pricingAvailabilities, CurrencyModel fromCurrency, CurrencyModel toCurrency, AvailabilityFilterModel? availability, string inquireOnlyString);
        Task<string> GetTotalCostOfStayLabelAsync(ListingViewModel listing, CurrencyModel fromCurrency, CurrencyModel toCurrency, AvailabilityFilterModel? availability, string inquireOnlyString, int adults, int pets);
        IEnumerable<PricingAvailabilityModel>? GetPricingAvailabilitiesBetweenDates<T>(T listing, AvailabilityFilterModel? availability) where T : IListingViewModel?;
        IEnumerable<PricingAvailabilityModel>? GetPricingAvailabilitiesBetweenDates(IEnumerable<PricingAvailabilityModel> pricingAvailabilities, AvailabilityFilterModel? availability);
        Task<decimal?> CalculateAverageCostOfStayAsync(IEnumerable<PricingAvailabilityModel>? pricingAvailabilities, CurrencyModel toCurrency, CurrencyModel fromCurrency, AvailabilityFilterModel? availability);
        Task<decimal?> CalculateTotalCostOfStayAsync(IListingViewModel listing, CurrencyModel toCurrency, CurrencyModel fromCurrency, AvailabilityFilterModel? availability, int adults, int pets);
    }
}
