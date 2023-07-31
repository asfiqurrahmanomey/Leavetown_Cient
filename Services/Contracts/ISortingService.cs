using Leavetown.Client.Models;
using Microsoft.Extensions.Primitives;

namespace Leavetown.Client.Services.Contracts
{
    public interface ISortingService
    {
        SortingOptions Parse(KeyValuePair<string, StringValues> query);
        List<SortingCriteria> GetAllOptions();
        string GetSortingQuery(SortingOptions sortingOptions);
    }
}
