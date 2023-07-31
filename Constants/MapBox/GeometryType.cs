using System.Text.Json.Serialization;

namespace Leavetown.Client.Constants.MapBox
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GeometryType
    {
        Point,
        Circle
    }
}
