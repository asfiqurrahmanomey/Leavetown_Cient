namespace Leavetown.Client.Clients.Contracts
{
    public interface IPricingAvailabilityClient
    {
        Task<IEnumerable<decimal>?> GetGroupedPricesFromListingsAsync();        
    }
}
