using Leavetown.Shared.Models.ViewModels;

namespace Leavetown.Client.Clients.Contracts
{
    public interface IListingClient
    {        
        Task<IEnumerable<ListingViewModel>?> GetListingModelsAsync(int? skip = null, int? top = null, string? where = null, string? order = null, bool featuredOnly = false);
        Task<int?> GetListingModelsCountAsync(string? where = null);
        Task<ListingDetailsViewModel?> GetSingleListingDetailsModelAsync(int id);        
    }
}
