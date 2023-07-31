using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Leavetown.Client.Models.ViewModels
{
    public class BookingViewModel : IQuoteListingParams
    {
        public int ListingId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Days => (CheckOut.Date - CheckIn.Date).Days;
        public int NumberOfAdults { get; set; }
        public int NumberOfChildren { get; set; }
        public int NumberOfPets { get; set; }
        public decimal ExpectedTotal { get; set; }
        public CancellationPolicyCode PolicyCode { get; set; }
        public string CurrencyCode { get; set; } = "";
        public bool PetsAllowed { get; set; }
        public int PetMax { get; set; } = 0;
        public string Query { get; set; } = "";
        public int StandardOccupants { get; set; }
        public decimal ServiceFeeRate { get; set; }
        public decimal ExtraAdultFee { get; set; }
        public decimal PetFeePerNight { get; set; }
        public decimal PetFeeMax { get; set; }
        public decimal CleaningFee { get; set; }
        public decimal AutohostFee { get; set; }
        public decimal TaxRate { get; set; }
        public decimal FlatTax { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone (inc. extension) is required")]
        public string PhoneNumber { get; set; } = "";

        [ValidateComplexType]
        public AddressDetails ContactAddressDetails { get; set; } = new();

        [ValidateComplexType]
        public AddressDetails? BillingAddressDetails { get; set; }
        public bool UseDifferentBillingAddress { get; set; } = false;
    }
}
