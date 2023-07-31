using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class PropertyTypeFilterService : IPropertyTypeFilterService
    {
        public string GetFilterQuery(PropertyTypeFilterModel filter)
        {
            if (filter.PropertyTypes == null) return "";

            StringBuilder stringBuilder = new();

            for (int i = 0; i < filter.PropertyTypes.Count; i++)
            {
                if (i != 0) { stringBuilder.Append("AND"); }
                PropertyTypeModel propertyType = filter.PropertyTypes[i];
                stringBuilder.Append($" property_type = '{propertyType.Name}' ");
            }

            return stringBuilder.ToString();
        }

        public PropertyTypeFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            PropertyTypeFilterModel filter = new();

            if (query.Key.Equals(nameof(FilterType.PropertyType).ToSnakeCase()) && !string.IsNullOrWhiteSpace(query.Value))
            {
                filter.PropertyTypes = query.Value.ToString().Split('|').Select(x => new PropertyTypeModel { Name = x }).ToList();
            }

            return filter;
        }
    }
}
