using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class LocationInfo
    {
        [Parameter, EditorRequired] public string ImageUrl { get; set; } = default!;
        [Parameter, EditorRequired] public string PropertyName { get; set; } = default!;
        [Parameter, EditorRequired] public string CityName { get; set; } = default!;
    }
}
