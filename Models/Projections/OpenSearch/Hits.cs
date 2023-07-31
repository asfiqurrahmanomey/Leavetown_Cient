using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections.OpenSearch
{
    public class Hits<T> where T : class
    {
        [JsonPropertyName("hits")]
        public List<Hit<T>>? HitList { get; set; }
    }
}
