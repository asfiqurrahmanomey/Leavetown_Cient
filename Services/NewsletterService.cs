using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services;

public class NewsletterService : INewsletterService
{
    private ILeavetownClient _leavetownClient;

    public NewsletterService(ILeavetownClient leavetownClient)
    {
        _leavetownClient = leavetownClient;
    }

    public async Task<bool> AddSubscriberAsync(ContactDetailsModel contactDetails) =>
        await _leavetownClient.AddNewsletterSubscriberAsync(contactDetails);
}
