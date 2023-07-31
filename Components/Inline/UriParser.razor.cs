using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Leavetown.Client.Components.Inline
{
    public partial class UriParser
    {
        [Inject] private ILeavetownClient LeavetownClient { get; set; } = default!;
        [Inject] private ILogger<UriParser> Logger { get; set; } = default!;

        [Parameter] public string? Culture { get; set; }

        public async Task<BookingViewModel> ParseQueryAsBookingAsync(Uri uri)
        {
            try
            {
                Dictionary<string, StringValues>? queryValues = QueryHelpers.ParseQuery(uri.Query);

                queryValues.TryGetValue(nameof(BookingViewModel.FirstName).ToLower(), out StringValues firstName);
                queryValues.TryGetValue(nameof(BookingViewModel.LastName).ToLower(), out StringValues lastName);
                queryValues.TryGetValue(nameof(BookingViewModel.ListingId).ToLower(), out StringValues listingIdValue);
                queryValues.TryGetValue(nameof(BookingViewModel.PhoneNumber).ToLower(), out StringValues phoneNumber);
                queryValues.TryGetValue(nameof(BookingViewModel.Email).ToLower(), out StringValues emailAddress);
                queryValues.TryGetValue(nameof(BookingViewModel.CheckIn).ToSnakeCase(), out StringValues checkInValue);
                queryValues.TryGetValue(nameof(BookingViewModel.CheckOut).ToSnakeCase(), out StringValues checkOutValue);
                queryValues.TryGetValue(nameof(BookingViewModel.ContactAddressDetails.Address1).ToLower(), out StringValues contactAddress1);
                queryValues.TryGetValue(nameof(BookingViewModel.ContactAddressDetails.Address2).ToLower(), out StringValues contactAddress2);
                queryValues.TryGetValue(nameof(BookingViewModel.ContactAddressDetails.City).ToLower(), out StringValues contactCity);
                queryValues.TryGetValue(nameof(BookingViewModel.ContactAddressDetails.ProvinceState).ToLower(), out StringValues contactProvinceState);
                queryValues.TryGetValue(nameof(BookingViewModel.ContactAddressDetails.PostalCode).ToLower(), out StringValues contactPostalCode);
                queryValues.TryGetValue("countrycode", out StringValues contactCountryCode);

                queryValues.TryGetValue(nameof(FilterType.Adults).ToLower(), out StringValues numberOfAdultsValue);
                queryValues.TryGetValue(nameof(FilterType.Children).ToLower(), out StringValues numberOfChildrenValue);
                queryValues.TryGetValue(nameof(FilterType.Pets).ToLower(), out StringValues numberOfPetsValue);

                IEnumerable<CountryModel> countries = (await LeavetownClient.GetCountryModelsAsync(Culture!))!;

                _ = DateTime.TryParse(checkInValue, out DateTime checkIn);
                _ = DateTime.TryParse(checkOutValue, out DateTime checkOut);
                _ = int.TryParse(listingIdValue, out int listingId);
                _ = int.TryParse(numberOfAdultsValue, out int numberOfAdults);
                _ = int.TryParse(numberOfChildrenValue, out int numberOfChildren);
                _ = int.TryParse(numberOfPetsValue, out int numberOfPets);

                CountryModel? contactCountry = countries!.SingleOrDefault(x => string.Equals(x.Code, contactCountryCode));

                var result = new BookingViewModel
                {
                    ListingId = listingId,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    Email = emailAddress,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    NumberOfAdults = numberOfAdults == 0 ? 1 : numberOfAdults,
                    NumberOfChildren = numberOfChildren,
                    NumberOfPets = numberOfPets,
                    ContactAddressDetails = new AddressDetails
                    {
                        Address1 = contactAddress1,
                        Address2 = contactAddress2,
                        City = contactCity,
                        Country = contactCountry ?? new(),
                        PostalCode = contactPostalCode,
                        ProvinceState = contactProvinceState
                    },
                    Query = uri.Query
                };

                result.BillingAddressDetails = result.ContactAddressDetails;

                return result;
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
