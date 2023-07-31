using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Microsoft.Extensions.Primitives;
using System.ComponentModel;
using System.Text;

namespace Leavetown.Client.Services
{
    public class SortFilterService : ISortingService
    {
        public string GetSortingQuery(SortingOptions sortingOptions)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(sortingOptions.SortingCriteria.SortQuery);

            return stringBuilder.ToString();
        }

        public SortingOptions Parse(KeyValuePair<string, StringValues> query) => new SortingOptions
        {
            SortingCriteria = GetAllOptions().Where(x => x.Tag == query.Value.ToString()).Single()
        };

        public List<SortingCriteria> GetAllOptions() => new()
        {
            SortingOptions.Relevance,
            SortingOptions.PriceLowToHigh,
            SortingOptions.PriceHighToLow,
            SortingOptions.SleepsLowToHigh,
            SortingOptions.SleepsHighToLow,
            SortingOptions.RecentlyAdded
        };
    }
}
