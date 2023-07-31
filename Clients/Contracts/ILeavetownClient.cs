using Leavetown.Client.Models.Projections;
using Leavetown.Shared.Email;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.PartnerApi;

namespace Leavetown.Client.Clients.Contracts;

public interface ILeavetownClient
{
    Task<IEnumerable<CountryModel>?> GetCountryModelsAsync(string cultureCode);
    Task<BookingResponseModel?> SendBookingRequestAsync(BookingRequestModel bookingRequest);
    Task<QuoteModel> SendQuoteRequestAsync(QuoteRequestModel quoteRequest);
    Task SendContactUsEmailRequestAsync(CustomerContactUsEmailModel emailModel);
    Task SendBusinessContactUsEmailRequestAsync(BusinessContactUsEmailModel emailModel);
    Task<bool> AddNewsletterSubscriberAsync(ContactDetailsModel contactDetails);
    Task<Dictionary<string, decimal>?> GetExchangeRatesAsync();
    Task<PaymentModel?> GetPayment(string paymentIntentId, string currencyCode);
}
