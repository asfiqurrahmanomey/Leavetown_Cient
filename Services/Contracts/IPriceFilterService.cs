using Leavetown.Client.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IPriceFilterService : IFilterService<PriceFilterModel>
    {
        Task<string> GetFilterQueryAsync(PriceFilterModel filter);
    }
}
