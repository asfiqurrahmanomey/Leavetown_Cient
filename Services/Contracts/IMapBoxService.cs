using Leavetown.Client.Constants.MapBox;
using Leavetown.Client.Models.MapBox;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IMapBoxService
    {
        LocationModel CurrentLocation { get; set; }
        event EventHandler MapFilter;

        Task<GeoJson> ConvertSingleListingAsync(string id, decimal longitude, decimal latitude, GeometryType geometryType, string? label = null);        

        // Javascript Function Wrappers

        /// <summary>
        /// Returns all HTML content within the element with the specified id
        /// </summary>
        /// <param name="id">Id of the HTML element to return</param>
        /// <returns>String representation of the HTML elements InnerHTML</returns>
        Task<string> GetInnerHtmlAsync(string id);

        /// <summary>
        /// Set HTML element ID
        /// </summary>
        /// <param name="oldId">ID of the element to change</param>
        /// <param name="newId">New ID of the element</param>
        /// <returns></returns>
        Task SetIdAsync(string oldId, string newId);

        /// <summary>
        /// Creates the MapBox "map" object. Does not set center or zoom
        /// </summary>
        /// <returns></returns>
        Task InitializeMapAsync();

        /// <summary>
        /// Sets the center for the map. If used in conjunction with <see cref="SetZoomLevelAsync"/>, the zoom must be set before the center.
        /// </summary>
        /// <param name="latitude">Latitude of the center point</param>
        /// <param name="longitude">Longitude of the center point</param>
        /// <param name="smoothTransition">Determines whether to smoothly "fly to" the center point upon setting</param>
        /// <returns></returns>
        Task SetCenterAsync(decimal latitude, decimal longitude, bool smoothTransition = false);

        /// <summary>
        /// Sets the zoom level for the map. If used in conjunction with <see cref="SetCenterAsync"/>, the zoom must be set before the center.
        /// <para />
        /// The following table provides a guideline for setting zoom levels. 
        /// This list is referenced from: <see href="https://docs.mapbox.com/help/glossary/zoom-level/">MapBox Zoom Level</see>
        /// <list type="table">
        ///     <listheader>
        ///         <term>At zoom level</term>
        ///         <term>You can see</term>
        ///     </listheader>
        ///     <item>
        ///         <term>0</term>
        ///         <term>The Earth</term>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <term>A continent</term>
        ///     </item>
        ///     <item>
        ///         <term>4</term>
        ///         <term>Large islands</term>
        ///     </item>
        ///     <item>
        ///         <term>6</term>
        ///         <term>Large rivers</term>
        ///     </item>
        ///     <item>
        ///         <term>10</term>
        ///         <term>Large roads</term>
        ///     </item>
        ///     <item>
        ///         <term>15</term>
        ///         <term>Buildings</term>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        Task SetZoomLevelAsync(int zoomLevel);

        /// <summary>
        /// Iteratively removes all markers from the markers collection
        /// </summary>
        /// <returns></returns>
        Task ClearMarkersAsync();

        /// <summary>
        /// Creates and sets the global listing markers.
        /// </summary>
        /// <param name="data">JSON serialized <see cref="GeoJson"/> object</param>
        /// <returns></returns>
        Task SetMarkersAsync(List<GeoJson> data);

        /// <summary>
        /// Creates and sets a red circle denoting an approximation of a listings location.
        /// </summary>
        /// <param name="data">JSON serialized <see cref="GeoJson"/> object</param>
        /// <returns></returns>
        Task SetCircleAsync(List<GeoJson> data);

        /// <summary>
        /// Calculates zoom level and center to fit all coordinates within the provided GeoJson string.
        /// </summary>
        /// <param name="data">JSON serialized <see cref="GeoJson"/> object</param>
        /// <param name="smoothTransition">Determines whether to smoothly "fly to" the center point upon setting</param>
        /// <returns></returns>
        Task FitToBoundsAsync(List<GeoJson> data, bool smoothTransition = true);

        /// <summary>
        /// Gets the bounding box of the map's current view in the form of two coordinates: The map's northeast coordinate, and the map's southwest coordinate.
        /// </summary>
        /// <returns></returns>
        Task<string> GetBoundsAsync();

        /// <summary>
        /// Sets the "on map dragend" event which will trigger <see cref="OnMapDragEnd"/> when the user finishes clicking and moving the map.
        /// </summary>
        /// <returns></returns>
        Task SetFiltersAsync();

        // ------------------------------ //
    }
}
