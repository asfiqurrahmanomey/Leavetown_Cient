using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IAmenityService
    {
        Task<List<AmenityModel>> GetAmenitiesAsync();
    }
}
