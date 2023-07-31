using Leavetown.Client.Models.Projections.OpenSearch;

namespace Leavetown.Client.Clients.Contracts
{
    public interface IOpenSearchClient
    {
        Task<IEnumerable<T>> GetDataAsync<T>(string query, int? size = null) where T : class;
        Task<int?> GetCountAsync<T>(string query) where T : class;
        Task<Aggregations?> GetAggregationsAsync<T>(string query) where T : class;
    }
}
