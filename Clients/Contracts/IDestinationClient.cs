using Leavetown.Client.Models.MapBox;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Clients.Contracts
{
    public interface IDestinationClient
    {
        Task<IEnumerable<DestinationModel>?> GetDestinationModelsAsync();
        Task<BoundingBox?> GetDestinationBoundingBox(string value);
    }
}
