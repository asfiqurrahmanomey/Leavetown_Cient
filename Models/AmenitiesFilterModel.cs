using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class AmenitiesFilterModel : IFilterable
    {
        public List<AmenityModel>? Amenities { get; set; } = new();
        public FilterType Type { get; } = FilterType.Amenities;
        public bool HasValue => !Equals(new AmenitiesFilterModel());
        public override bool Equals(object? obj) => obj != null && Equals((AmenitiesFilterModel)obj);
        private bool Equals(AmenitiesFilterModel amenitiesFilterModel)
        {
            if (Amenities == null || amenitiesFilterModel == null) return false;

            return Amenities.Equals(amenitiesFilterModel.Amenities);
        }

        public override int GetHashCode() => HashCode.Combine(Amenities);
    }
}
