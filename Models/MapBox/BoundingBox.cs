using Leavetown.Shared.Models;
using System.Text.Json.Serialization;
using System.Web;

namespace Leavetown.Client.Models.MapBox
{
    public class BoundingBox
    {
        public double MaxLatitude { get; set; } = 0.0;
        public double MinLatitude { get; set; } = 0.0;
        public double MaxLongitude { get; set; } = 0.0;
        public double MinLongitude { get; set; } = 0.0;

        public LocationModel? GetNorthEastPoint() => new((decimal)MaxLatitude, (decimal)MaxLongitude);
        public LocationModel? GetSouthWestPoint() => new((decimal)MinLatitude, (decimal)MinLongitude);

        public BoundingBox() { }

        public BoundingBox(LocationModel southWestPoint, LocationModel northEastPoint)
        {
            MaxLatitude = (double)northEastPoint.Lat;
            MaxLongitude = (double)northEastPoint.Lon;
            MinLatitude = (double)southWestPoint.Lat;
            MinLongitude = (double)southWestPoint.Lon;
        }
        public decimal[] CoordinateArray => new decimal[] { GetNorthEastPoint()?.Lat ?? 0, GetNorthEastPoint()?.Lon ?? 0, GetSouthWestPoint()?.Lat ?? 0, GetSouthWestPoint()?.Lon ?? 0 };

        public bool HasValue => (MaxLatitude != 0 && MaxLongitude != 0 && MinLatitude != 0 && MinLongitude != 0);

        public override string ToString() => GetNorthEastPoint() != null && GetSouthWestPoint() != null ?
            $"[[{GetNorthEastPoint()?.Lon},{GetNorthEastPoint()?.Lat}],[{GetSouthWestPoint()?.Lon},{GetSouthWestPoint()?.Lat}]]" :
            throw new ArgumentNullException("NorthEast and SouthWest points are not set. Unable to convert to string.");

        public override bool Equals(object? obj) => obj != null && Equals((BoundingBox)obj);

        public override int GetHashCode() => HashCode.Combine(GetNorthEastPoint(), GetSouthWestPoint());

        private bool Equals(BoundingBox boundingBox)
        {
            if (boundingBox == null) return false;

            return Equals(GetNorthEastPoint(), boundingBox.GetNorthEastPoint()) &&
                Equals(GetSouthWestPoint(), boundingBox.GetSouthWestPoint());
        }
    }
}
