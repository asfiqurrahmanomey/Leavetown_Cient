namespace Leavetown.Client.Utilities.Settings
{
    public class Configuration
    {
        public OpenSearchSettings OpenSearch { get; set; } = new();

        public LeavetownApiSettings LeavetownApi { get; set; } = new();
        public StripeSettings Stripe { get; set; } = new();
        public WhiteLabelSettings WhiteLabelSettings { get; set; } = new();

        public string SalesChannel { get; set; } = string.Empty;

        public string[] CurrencyCodesToShowQuoteTotal { get; set; } = Array.Empty<string>();
    }
}
