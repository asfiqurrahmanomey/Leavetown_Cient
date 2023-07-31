using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Web;

namespace Leavetown.Client.Services
{
    public class LocationFilterService : ILocationFilterService
    {
        public LocationFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            LocationFilterModel filter = new();

            var values = HttpUtility.UrlDecode(query.Value.ToString()).Split('~');

            if (values.Length > 1)
            {
                List<string> boundingCoordinates = values[1].Trim(new char[] { '[', ']' }).Split("],[").ToList();

                if (boundingCoordinates.Count != 2) throw new Exception($"Query string cannot be parsed into bounding box. 2 coordinates are required, but {boundingCoordinates.Count} coordinate(s) were found.");

                filter.BoundingBox.MaxLatitude = (double)ParseCornerCoordinate(boundingCoordinates[0]).Lat;
                filter.BoundingBox.MaxLongitude = (double)ParseCornerCoordinate(boundingCoordinates[0]).Lon;
                filter.BoundingBox.MinLatitude = (double)ParseCornerCoordinate(boundingCoordinates[1]).Lat;
                filter.BoundingBox.MinLongitude = (double)ParseCornerCoordinate(boundingCoordinates[1]).Lon;

                LocationModel ParseCornerCoordinate(string coordinate)
                {
                    var coordinates = HttpUtility.UrlDecode(coordinate).Split(',');
                    var lat = decimal.Parse(coordinates[0].Trim(new char[] { '[', ']' }));
                    var lon = decimal.Parse(coordinates[1].Trim(new char[] { '[', ']' }));
                    return new LocationModel(lat, lon);
                }
            }

            filter.Name = values[0];

            return filter;
        }

        public string GetFilterQuery(LocationFilterModel filter)
        {
            StringBuilder stringBuilder = new();
            
            if (Equals(filter.Name.ToLower(), "map view"))
            {
                if (filter.BoundingBox.GetSouthWestPoint() != null && filter.BoundingBox.GetNorthEastPoint() != null)
                {
                    stringBuilder.Append($"(latitude >= {filter.BoundingBox.GetSouthWestPoint()?.Lat}");
                    stringBuilder.Append($" AND");
                    stringBuilder.Append($" latitude <= {filter.BoundingBox.GetNorthEastPoint()?.Lat}");
                    stringBuilder.Append($" AND");
                    stringBuilder.Append($" longitude >= {filter.BoundingBox.GetSouthWestPoint()?.Lon}");
                    stringBuilder.Append($" AND");
                    stringBuilder.Append($" longitude <= {filter.BoundingBox.GetNorthEastPoint()?.Lon})");
                }
            }
            else
            {
                stringBuilder.Append($"(country.normalized_name = '{filter.Name.ToLower()}'");
                stringBuilder.Append($" OR");
                stringBuilder.Append($" normalized_region_name = '{filter.Name.ToLower()}'");
                stringBuilder.Append($" OR");
                stringBuilder.Append($" normalized_destination_name = '{filter.Name.ToLower()}')");
            }

            return stringBuilder.ToString();
        }
    }
}
