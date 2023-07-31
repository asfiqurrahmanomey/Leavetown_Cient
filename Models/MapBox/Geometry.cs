using Leavetown.Client.Constants.MapBox;
using System.Text.Json.Serialization;

namespace Leavetown.Client.Models.MapBox
{
    public class Geometry
    {
        public Geometry() { }

        [JsonPropertyName("type")]
        public string TypeString => Type.ToString();
        [JsonIgnore]
        public GeometryType Type { get; set; } = GeometryType.Point;
        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new();
    }
}
