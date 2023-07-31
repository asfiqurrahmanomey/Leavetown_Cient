using Leavetown.Client.Models.Stripe;
using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Projections
{
    public class StripeCreateCardResponse
    {
        [JsonPropertyName("token")]
        public StripeToken? Token { get; set; }

        [JsonPropertyName("error")]
        public StripeError? Error { get; set; }
    }
}
