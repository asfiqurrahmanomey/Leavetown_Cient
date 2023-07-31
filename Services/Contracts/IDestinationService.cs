using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IDestinationService
    {
        Task<List<DestinationModel>> GetDestinationsAsync();
    }
}
