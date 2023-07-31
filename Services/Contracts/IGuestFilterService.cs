using Leavetown.Client.Models;
using Microsoft.Extensions.Primitives;

namespace Leavetown.Client.Services.Contracts
{
    public interface IGuestFilterService : IFilterService<GuestFilterModel>
    {
        Task<GuestFilterModel> ParseAsync(Dictionary<string, StringValues> queryValues);
    }
}
