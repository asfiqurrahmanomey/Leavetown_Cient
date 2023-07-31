using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Clients
{
    public class PropertyTypeClient : IPropertyTypeClient
    {
        private IOpenSearchClient _openSearchClient;

        public PropertyTypeClient(IOpenSearchClient openSearchClient)
        {
            _openSearchClient = openSearchClient;
        }

        public async Task<IEnumerable<PropertyTypeModel>?> GetPropertyTypeModelsAsync() =>
            await _openSearchClient.GetDataAsync<PropertyTypeModel>("SELECT * FROM property-types");
    }
}
