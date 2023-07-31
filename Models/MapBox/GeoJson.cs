using Leavetown.Client.Constants.MapBox;
using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.MapBox
{
    public class GeoJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";
        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; } = new();
        [JsonPropertyName("properties")]
        public GeoProperty GeoProperty { get; set; } = new();

        public GeoJson() { }

        public GeoJson(List<double> coordinates, string description, string? text,
            GeometryType geometryType, string id)
        {
            GeoProperty = new GeoProperty
            {
                Description = description,
                Title = text,
                Id = id
            };
            Type = "Feature";
            Geometry = new Geometry
            {
                Type = geometryType,
                Coordinates = coordinates
            };
        }
    }
}
