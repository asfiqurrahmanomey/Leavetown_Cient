using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class GroupByAggregation
    {
        [JsonPropertyName("buckets")]
        public IEnumerable<JsonObject> Buckets { get; set; } = Enumerable.Empty<JsonObject>();
    }
}
