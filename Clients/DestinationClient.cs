using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models.MapBox;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;

namespace Leavetown.Client.Clients
{
    public class DestinationClient : IDestinationClient
    {
        private readonly HttpClient _httpClient;
       
        public DestinationClient(HttpClient httpClient)
        {
            _httpClient = httpClient;            
        }

        public Task<IEnumerable<DestinationModel>?> GetDestinationModelsAsync()
        {
            return _httpClient.GetFromJsonAsync<IEnumerable<DestinationModel>?>("api/destinations");
        }

        public async Task<BoundingBox?> GetDestinationBoundingBox(string value)
        {
            if (string.IsNullOrEmpty(value)) return new();
            var url = QueryHelpers.AddQueryString("api/destinations/bounds", "value", value);
            return await _httpClient.GetFromJsonAsync<BoundingBox?>(url);
        }
    }
}
