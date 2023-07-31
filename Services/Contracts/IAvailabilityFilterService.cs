using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IAvailabilityFilterService : IFilterService<AvailabilityFilterModel>
    {
        AvailabilityFilterModel Parse(string checkIn, string checkOut);
    }
}
