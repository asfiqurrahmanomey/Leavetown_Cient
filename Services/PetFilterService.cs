using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class PetFilterService : IPetFilterService
    {

        public PetFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            PetFilterModel filter = new();
            string queryValue = query.Value.ToString();

            if (string.IsNullOrEmpty(queryValue)) return filter;

            filter.PetCount = Convert.ToInt32(queryValue);

            return filter;
        }

        public string GetFilterQuery(PetFilterModel filter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"(number_of_pets is null");
            stringBuilder.Append($" OR");
            stringBuilder.Append($" number_of_pets >= {filter.PetCount})");

            return stringBuilder.ToString();
        }
    }
}
