using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class Hit<T> where T : class
    {
        [JsonPropertyName("_source")]
        public T? Source { get; set; }
    }
}
