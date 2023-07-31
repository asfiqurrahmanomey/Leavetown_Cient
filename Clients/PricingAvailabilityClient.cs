using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models.ViewModels;
using System.Text;

namespace Leavetown.Client.Clients
{
    public class PricingAvailabilityClient : IPricingAvailabilityClient
    {
        private IOpenSearchClient _openSearchClient;

        public PricingAvailabilityClient(IOpenSearchClient openSearchClient)
        {
            _openSearchClient = openSearchClient;
        }

        public async Task<IEnumerable<decimal>?> GetGroupedPricesFromListingsAsync()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"SELECT pricing_availabilities.price AS price FROM listings l, l.pricing_availabilities GROUP BY price");
            return (await _openSearchClient.GetAggregationsAsync<ListingViewModel>(stringBuilder.ToString()))?
                .Prices.Nested.Buckets.Select(x => x["key"]?.GetValue<decimal>() ?? 0M);
        }       
    }
}
