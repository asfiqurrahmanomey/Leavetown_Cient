using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Clients
{
    public class AmenityClient : IAmenityClient
    {
        private IOpenSearchClient _openSearchClient;

        public AmenityClient(IOpenSearchClient openSearchClient)
        {
            _openSearchClient = openSearchClient;
        }

        public async Task<IEnumerable<AmenityModel>?> GetAmenityModelsAsync() =>
            await _openSearchClient.GetDataAsync<AmenityModel>("SELECT * FROM amenities");
    }
}
