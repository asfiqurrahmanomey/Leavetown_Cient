using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface INewsletterService
    {
        Task<bool> AddSubscriberAsync(ContactDetailsModel contactDetails);
    }
}
