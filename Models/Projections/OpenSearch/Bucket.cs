using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class Bucket<T>
    {
        [JsonPropertyName("key")]
        public T Key { get; set; } = default!;
    }
}
