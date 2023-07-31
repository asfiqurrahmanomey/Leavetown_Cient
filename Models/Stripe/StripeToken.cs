using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Stripe;

public class StripeToken
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("card")]
    public CreditCard? CreditCard { get; set; }

    [JsonPropertyName("client_ip")]
    public string? ClientIp { get; set; }

    [JsonPropertyName("created")]
    public int? Created { get; set; }

    [JsonPropertyName("livemode")]
    public bool? Livemode { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("used")]
    public bool? Used { get; set; }
}


