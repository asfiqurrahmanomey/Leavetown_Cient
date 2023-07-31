using Leavetown.Client.Clients.Contracts;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;

namespace Leavetown.Client.Clients
{
    public class ListingClient : IListingClient
    {
        private readonly HttpClient _httpClient;
        
        public ListingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ListingViewModel>?> GetListingModelsAsync(int? skip = null, int? top = null, string? where = null, string? order = null, bool featuredOnly = false)
        {
            var queryStringParameters = new Dictionary<string, string>();
            if (skip != null) queryStringParameters["skip"] = $"{skip}";
            if (top != null) queryStringParameters["top"] = $"{top}";
            if (order != null) queryStringParameters["order"] = $"{order}";
            if (featuredOnly) queryStringParameters["featuredOnly"] = $"{featuredOnly}";
            var url = QueryHelpers.AddQueryString("api/listings", queryStringParameters);
            var response = await _httpClient.PostAsJsonAsync(url, where ?? "");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ListingViewModel>?>();
        }

        public async Task<int?> GetListingModelsCountAsync(string? where = null)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/listings/count", where ?? "");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int?>();            
        }

        public Task<ListingDetailsViewModel?> GetSingleListingDetailsModelAsync(int id)
        {
            return _httpClient.GetFromJsonAsync<ListingDetailsViewModel?>($"api/listings/{id}/details");
        }         
    }
}
