using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels.Contracts;

namespace Leavetown.Client.Services.Contracts
{
    public interface IQuoteService
    {
        Task<QuoteModel> UpdateQuoteAsync(int listingId, IQuoteListingParams listingParams, DateTime checkInTime, DateTime checkOutTime, int adultCount, int childCount, int petCount);

        decimal CalculateQuoteTotal(decimal rentalTotal, IQuoteListingParams listingParams, int nights, int adults, int pets);        
    }
}
