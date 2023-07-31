using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Constants;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class BedroomFilterService : IBedroomFilterService
    {
        public string GetFilterQuery(BedroomFilterModel filter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($" number_of_bedrooms >= {filter.BedroomCount} ");
            return stringBuilder.ToString();
        }

        public BedroomFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            BedroomFilterModel filter = new();

            if (query.Key.Equals(nameof(FilterType.Bedroom).ToLower()) && !string.IsNullOrWhiteSpace(query.Value.ToString()))
            {
                filter.BedroomCount = Convert.ToInt32(query.Value.ToString());
            }

            return filter;
        }
    }
}
