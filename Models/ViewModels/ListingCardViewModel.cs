using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;

namespace Leavetown.Client.Models.ViewModels
{
    public class ListingCardViewModel
    {
        public ListingViewModel Listing { get; set; } = default!;
        public AvailabilityFilterModel? Availability { get; set; } = default!;
        public GuestFilterModel? Guests { get; set; } = default!;
        public PetFilterModel? Pets { get; set; } = default!;
        public string PriceLabel { get; set; } = default!;

        public bool IsDataProvidedForBooking => 
            (!string.IsNullOrEmpty(PriceLabel)) &&
            (Availability?.HasValue ?? false) && 
            ((Guests?.HasValue ?? false) || (Pets?.HasValue ?? false));

        public bool Equals(ListingCardViewModel? another)
        {
            if(another == null) return false;
            if (!Listing.Equals(another.Listing)) return false;
            if (!Availability?.Equals(another.Availability) ?? another.Availability != null) return false;
            if (!Guests?.Equals(another.Guests) ?? another.Guests != null) return false;
            if (!Pets?.Equals(another.Pets) ?? another.Pets != null) return false;
            if (!string.Equals(PriceLabel, another.PriceLabel)) return false;

            return true;
        }
    }
}
