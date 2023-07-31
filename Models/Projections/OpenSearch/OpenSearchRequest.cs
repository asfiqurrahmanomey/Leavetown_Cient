using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class OpenSearchRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; } = "";
        [JsonPropertyName("fetch_size")]
        public int? FetchSize { get; set; }
    }
}
