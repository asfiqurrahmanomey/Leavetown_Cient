using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Microsoft.Extensions.Primitives;
using Radzen;
using System.Text;
using System.Web;

namespace Leavetown.Client.Services
{
    public class AmenitiesFilterService : IAmenitiesFilterService
    {
        public string GetFilterQuery(AmenitiesFilterModel filter)
        {
            if (filter.Amenities == null) return "";

            StringBuilder stringBuilder = new();

            for(int i = 0; i < filter.Amenities.Count; i ++)
            {
                if(i != 0) { stringBuilder.Append("AND"); }
                AmenityModel amenity = filter.Amenities[i];
                // Filtering by "name" due to the constraints of how we build our filter models. Filtered
                //  data is pulled from the url query and finding the id would require a request to the DB.
                stringBuilder.Append($" amenities.name = '{amenity.Name}' ");
            }

            return stringBuilder.ToString();
        }

        public AmenitiesFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            AmenitiesFilterModel filter = new();
            if (query.Key.Equals(nameof(FilterType.Amenities).ToLower()) && !string.IsNullOrWhiteSpace(query.Value))
            {
                filter.Amenities = query.Value.ToString().Split('|').Select(a => new AmenityModel { Name = HttpUtility.UrlDecode(a) } ).ToList();
            }

            return filter;
        }
    }
}
