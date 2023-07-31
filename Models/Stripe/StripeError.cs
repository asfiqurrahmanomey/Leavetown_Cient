using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Stripe
{
    public class StripeError
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
