using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models.Projections.OpenSearch;
using Radzen;
using System.Net.Http.Json;

namespace Leavetown.Client.Clients
{
    public class OpenSearchClient : IOpenSearchClient
    {
        private readonly HttpClient _httpClient;

        public OpenSearchClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<T>> GetDataAsync<T>(string query, int? size = null) where T : class => 
            (await PostAsync<T>(query)).Hits?.HitList?
                .Where(x => x.Source != null)
                .Select(x => x.Source!) ?? Enumerable.Empty<T>();

        public async Task<int?> GetCountAsync<T>(string query) where T : class => (await GetAggregationsAsync<T>(query))?.Count?.Value;

        public async Task<Aggregations?> GetAggregationsAsync<T>(string query) where T : class => (await PostAsync<T>(query)).Aggregations;

        private async Task<OpenSearchResponse<T>> PostAsync<T>(string query, int? size = null) where T : class
        {
            var uri = new Uri($"{_httpClient.BaseAddress}");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            httpRequestMessage.Content = size == null ?
                JsonContent.Create(new { Query = query }) :
                JsonContent.Create(new OpenSearchRequest { Query = query, FetchSize = size });
            HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
            return await response.ReadAsync<OpenSearchResponse<T>>();
        }
    }
}
