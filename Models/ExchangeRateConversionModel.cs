namespace Leavetown.Client.Models
{
    public class ExchangeRateConversionModel
    {
        public decimal To { get; set; }
        public decimal From { get; set; }

        public ExchangeRateConversionModel(decimal to, decimal from)
        {
            To = to;
            From = from;
        }
    }
}
