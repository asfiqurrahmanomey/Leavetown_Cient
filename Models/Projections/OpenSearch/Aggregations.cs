using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{

    public class Aggregations
    {
        [JsonPropertyName("count")]
        public CountAggregation<int>? Count { get; set; }

        [JsonPropertyName("max_latitude")]
        public CountAggregation<double?>? MaxLatitude { get; set; }

        [JsonPropertyName("max_longitude")]
        public CountAggregation<double?>? MaxLongitude { get; set; }

        [JsonPropertyName("min_latitude")]
        public CountAggregation<double?>? MinLatitude { get; set; }

        [JsonPropertyName("min_longitude")]
        public CountAggregation<double?>? MinLongitude { get; set; }

        [JsonPropertyName("pricing_availabilities.price@NESTED")]
        public PriceAggregation Prices { get; set; } = new();

        [JsonPropertyName("country_name")]
        public GroupByAggregation Destinations { get; set; } = new();

        [JsonPropertyName("distinct_ids")]
        public GroupByAggregation DistinctIds { get; set; } = new();

        public bool HasValue =>
            (Count?.Value != null) ||
            (MaxLatitude?.Value != null && MaxLongitude?.Value != null &&
            MinLongitude?.Value != null && MinLatitude?.Value != null) ||
            (Prices.Nested.Buckets.Any()) || (Destinations.Buckets.Any());
    }

    public class PriceAggregation
    {
        [JsonPropertyName("price")]
        public GroupByAggregation Nested { get; set; } = new();

    }
}
