using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.Stripe;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class CreditCard
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("address_city")]
    public object? AddressCity { get; set; }

    [JsonPropertyName("address_country")]
    public object? AddressCountry { get; set; }

    [JsonPropertyName("address_line1")]
    public object? AddressLine1 { get; set; }

    [JsonPropertyName("address_line1_check")]
    public object? AddressLine1Check { get; set; }

    [JsonPropertyName("address_line2")]
    public object? AddressLine2 { get; set; }

    [JsonPropertyName("address_state")]
    public object? AddressState { get; set; }

    [JsonPropertyName("address_zip")]
    public string? AddressZip { get; set; }

    [JsonPropertyName("address_zip_check")]
    public string? AddressZipCheck { get; set; }

    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("cvc_check")]
    public string? CvcCheck { get; set; }

    [JsonPropertyName("dynamic_last4")]
    public object? DynamicLast4 { get; set; }

    [JsonPropertyName("exp_month")]
    public int? ExpMonth { get; set; }

    [JsonPropertyName("exp_year")]
    public int? ExpYear { get; set; }

    [JsonPropertyName("funding")]
    public string? Funding { get; set; }

    [JsonPropertyName("last4")]
    public string? Last4 { get; set; }

    [JsonPropertyName("name")]
    public object? Name { get; set; }

    [JsonPropertyName("tokenization_method")]
    public object? TokenizationMethod { get; set; }
}


