namespace Leavetown.Client.Utilities.Settings
{
    public class StripeSettings
    {
        public static string CanadaApiKey { get; set; } = "";
        public static string EuropeApiKey { get; set; } = "";
        public static string UnitedKingdomApiKey { get; set; } = "";
        public static string UnitedStatesApiKey { get; set; } = "";

        public static string GetStripeApiKey(string currencyCode) => (currencyCode) switch
        {
            "CAD" => CanadaApiKey,
            "USD" => UnitedStatesApiKey,
            "EUR" => EuropeApiKey,
            "GBP" => UnitedKingdomApiKey,
            _ => throw new Exception($"Failed to retrieve Stripe Api Key with Currency Code: {currencyCode}")
        };
    }
}
