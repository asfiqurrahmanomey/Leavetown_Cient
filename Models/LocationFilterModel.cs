using Leavetown.Client.Models.MapBox;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;
using System.Web;

namespace Leavetown.Client.Models
{
    public class LocationFilterModel : IFilterable
    {
        public FilterType Type { get; } = FilterType.Location;
        public string Name { get; set; } = "";
        public BoundingBox BoundingBox { get; set; } = new BoundingBox();

        public bool IsFilteringViaMap => string.Equals(Name.ToLower(), "map view");

        public string Value => HasValue ? 
            BoundingBox.HasValue ? HttpUtility.UrlEncode($"{Name}~{BoundingBox}") :
            HttpUtility.UrlEncode($"{Name}") :
            "";

        public bool HasValue => !Equals(new LocationFilterModel()) || BoundingBox.HasValue;

        public bool Equals(LocationFilterModel location)
        {
            if(location == null) return false;

            return Name.Equals(location.Name) && BoundingBox.Equals(location.BoundingBox);
        }
    }
}
