using Leavetown.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Leavetown.Client.Models
{
    public class AddressDetails
    {

        [Required(ErrorMessage = "Address line 1 is required")]
        public string Address1 { get; set; } = "";

        public string Address2 { get; set; } = "";

        [Required(ErrorMessage = "Town / city is required")]
        public string City { get; set; } = "";

        public string ProvinceState { get; set; } = "";

        [Required(ErrorMessage = "Zip / postal code is required")]
        public string PostalCode { get; set; } = "";

        [ValidateComplexType]
        public CountryModel Country { get; set; } = new();

        public AddressDetails ShallowCopy()
        {
            return (AddressDetails)MemberwiseClone();
        }
    }
}
