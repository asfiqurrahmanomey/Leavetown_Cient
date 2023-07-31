using Leavetown.Shared.Models;

namespace Leavetown.Client.Clients.Contracts
{
    public interface IAmenityClient
    {
        Task<IEnumerable<AmenityModel>?> GetAmenityModelsAsync();
    }
}
