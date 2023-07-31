using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class CountAggregation<T>
    {
        [JsonPropertyName("value")]
        public T Value { get; set; } = default!;
    }
}
