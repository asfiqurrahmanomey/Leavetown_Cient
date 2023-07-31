using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models.Projections;
using Leavetown.Shared.Clients;
using Leavetown.Shared.Email;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.PartnerApi;
using System.Net.Http.Json;

namespace Leavetown.Client.Clients;

public class LeavetownClient : ApiClient, ILeavetownClient
{
    private ILogger<LeavetownClient> _logger;

    public LeavetownClient(HttpClient httpClient, ILogger<LeavetownClient> logger) : base(httpClient)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<CountryModel>?> GetCountryModelsAsync(string cultureCode)
    {
        try
        {
            return await HttpClient.GetFromJsonAsync<List<CountryModel>>($"data/countries.{cultureCode}.json");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<PaymentModel?> GetPayment(string paymentIntentId, string currencyCode)
    {
        try
        {
            return await GetAsync<PaymentModel?>($"api/payment?paymentIntentId={paymentIntentId}&currencyCode={currencyCode}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<BookingResponseModel?> SendBookingRequestAsync(BookingRequestModel bookingRequest)
    {
        try
        {
            return await PostAsync<BookingRequestModel, BookingResponseModel>("api/bookings", bookingRequest);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<QuoteModel> SendQuoteRequestAsync(QuoteRequestModel quoteRequest)
    {
        try
        {
            QuoteModel? quote = await PostAsync<QuoteRequestModel, QuoteModel>("api/bookings/quote", quoteRequest, TimeSpan.FromSeconds(10));

            if (quote == null) return new QuoteModel
            {
                BookingErrorCodes = new string[] { "BookingErrorQuoteFailed" }
            };

            return quote;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new QuoteModel
            {
                BookingErrorCodes = new string[]
            {
                ex switch
                {
                    HttpRequestException => "BookingErrorHttpRequest",
                    TimeoutException => "BookingErrorRequestTimeout",
                    _ => "BookingErrorGeneric"
                }}
            };
        }
    }

    public async Task SendContactUsEmailRequestAsync(CustomerContactUsEmailModel emailModel)
    {
        try
        {
            await PostAsync("api/email/customer", emailModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task SendBusinessContactUsEmailRequestAsync(BusinessContactUsEmailModel emailModel)
    {
        try
        {
            await PostAsync("api/email/business", emailModel);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> AddNewsletterSubscriberAsync(ContactDetailsModel contactDetails)
    {
        try
        {
            return await PostAsync<ContactDetailsModel, bool>("api/newsletter", contactDetails);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<Dictionary<string, decimal>?> GetExchangeRatesAsync()
    {
        try
        {
            return await GetAsync<Dictionary<string, decimal>?>("api/currencyexchange");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
