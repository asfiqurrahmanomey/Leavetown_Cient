using System.Text.Json.Serialization;

namespace Leavetown.Client.Models
{
    public class CacheItem<T> where T : class
    {
        [JsonPropertyName("data")]
        public T Data { get; set; } = default!;
        [JsonPropertyName("expiry_timestamp")]
        public DateTime ExpiryTimeStamp { get; set; }
    }
}
