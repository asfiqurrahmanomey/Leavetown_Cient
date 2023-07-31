using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class OpenSearchResponse<T> where T : class
    {
        [JsonPropertyName("hits")]
        public Hits<T>? Hits { get; set; }

        [JsonPropertyName("aggregations")]
        public Aggregations? Aggregations { get; set; }
    }
}
