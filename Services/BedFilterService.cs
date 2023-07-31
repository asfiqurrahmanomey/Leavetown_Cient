using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Constants;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class BedFilterService : IBedFilterService
    {

        public string GetFilterQuery(BedFilterModel filter)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"(number_of_beds is null");
            stringBuilder.Append($" OR");
            stringBuilder.Append($" number_of_beds >= {filter.BedCount})");

            return stringBuilder.ToString();
        }

        public BedFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            BedFilterModel filter = new();

            string queryValue = query.Value.ToString();

            if (string.IsNullOrEmpty(queryValue)) return filter;

            filter.BedCount = Convert.ToInt32(queryValue);

            return filter;
        }
    }
}
