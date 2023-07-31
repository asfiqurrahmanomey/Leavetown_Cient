using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Constants;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class BathroomFilterService : IBathroomFilterService
    {
        public string GetFilterQuery(BathroomFilterModel filter)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($" number_of_bathrooms >= {(decimal)filter.BathroomCount}");
            return stringBuilder.ToString();
        }

        public BathroomFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            BathroomFilterModel filter = new();

            if (query.Key.Equals(nameof(FilterType.Bathroom).ToLower()) && !string.IsNullOrWhiteSpace(query.Value.ToString()))
            {
                filter.BathroomCount = Convert.ToInt32(query.Value.ToString());
            }

            return filter;
        }
    }
}
