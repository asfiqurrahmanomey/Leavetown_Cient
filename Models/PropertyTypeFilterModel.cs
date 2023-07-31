using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class PropertyTypeFilterModel : IFilterable
    {
        public List<PropertyTypeModel>? PropertyTypes { get; set; } = new();
        public FilterType Type { get; } = FilterType.PropertyType;
        public bool HasValue => !Equals(new PropertyTypeFilterModel());
        public override bool Equals(object? obj) => obj != null && Equals((PropertyTypeFilterModel)obj);
        private bool Equals(PropertyTypeFilterModel propertyTypeFilterModel)
        {
            if (PropertyTypes == null || propertyTypeFilterModel == null) return false;

            return PropertyTypes.Equals(propertyTypeFilterModel.PropertyTypes);
        }

        public override int GetHashCode() => HashCode.Combine(PropertyTypes?.Select(x => x.Name));
    }
}
