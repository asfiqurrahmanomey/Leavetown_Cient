using Leavetown.Shared.Models;

namespace Leavetown.Client.Models.Events
{
    public class CurrencyChangedEventArgs : EventArgs
    {
        public CurrencyModel CurrencyModel { get; set; } = new();

        public CurrencyChangedEventArgs(CurrencyModel currency)
        {
            CurrencyModel = currency;
        }
    }
}
