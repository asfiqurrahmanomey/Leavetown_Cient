using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;


namespace Leavetown.Client.Models
{
    public class GuestFilterModel : IFilterable
    {
        public FilterType Type { get; } = FilterType.Guests;
        public int AdultCount
        {
            get => _adultCount;
            set => _adultCount = value < MinAdults ? MinAdults : value;
        }

        public int ChildCount { get; set; } = 0;
        public int GuestCount
        {
            get => _guestCount;
            set => _guestCount = value < MinAdults ? MinAdults : value;
        }
        public int MinAdults { get; set; } = 1;

        private int _adultCount = 0;
        private int _guestCount = 0;

        public GuestFilterModel()
        {
            _adultCount = MinAdults;
            _guestCount = MinAdults;
        }

        public bool HasValue => AdultCount != 0;

        public override bool Equals(object? obj) => obj != null && Equals((GuestFilterModel)obj);

        public override int GetHashCode() => HashCode.Combine(GuestCount, AdultCount, ChildCount);

        private bool Equals(GuestFilterModel? guestTypeAmount)
        {
            if (guestTypeAmount == null) return false;

            return 
                GuestCount == guestTypeAmount.GuestCount &&
                AdultCount == guestTypeAmount.AdultCount && 
                ChildCount == guestTypeAmount.ChildCount;
        }
    }
}
