using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.MapBox
{
    public class GeoProperty
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
    }
}
