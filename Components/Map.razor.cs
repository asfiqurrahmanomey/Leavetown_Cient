using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Models.MapBox;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components
{
    public partial class Map : IDisposable
    {
        [Inject] private IMapBoxService MapBoxService { get; set; } = default!;
        [Inject] private IDestinationClient DestinationClient { get; set; } = default!;

        [Parameter] public bool SingleLocation { get; set; } = false;
        [Parameter] public EventCallback<BoundingBox> OnMapViewChanged { get; set; }
        [Parameter] public List<GeoJson>? Data { get; set; }

        private bool _isMapDragged = false;
        private bool _isMapInitialized = false;
        private bool disposedValue;

        protected override void OnInitialized()
        {
            MapBoxService.MapFilter += OnMapFilter;
        }

        public async Task InitializeMapAsync()
        {
            await MapBoxService.InitializeMapAsync();
            if (!SingleLocation)
            {
                await MapBoxService.SetFiltersAsync();
            }
        }

        private void OnMapFilter(object? sender, EventArgs e)
        {
            InvokeAsync(async () => {
                var boundBoxString = await MapBoxService.GetBoundsAsync();
                var boundingBoxArray = boundBoxString.Split(',').Select(x => decimal.Parse(x)).ToArray();

                if (boundingBoxArray.Length != 4) throw new Exception($"Bounding box does not contain the right amount of coordinates to be formed. Number of coordinates should be 4 but {boundingBoxArray.Length} were found.");

                BoundingBox boundingBox = new(
                    new LocationModel(boundingBoxArray[0], boundingBoxArray[1]),
                    new LocationModel(boundingBoxArray[2], boundingBoxArray[3])
                );

                _isMapDragged = true;
                await OnMapViewChanged.InvokeAsync(boundingBox);
            });
        }

        public async Task UpdateMapAsync(List<GeoJson>? geoJsonData = null, string? locationValue = null)
        {
            geoJsonData ??= Data;
            if (!_isMapInitialized) await InitializeMapAsync();
            if (geoJsonData != null && geoJsonData.Any())
            {
                await MapBoxService.ClearMarkersAsync();

                if (SingleLocation) await UpdateSingleLocationAsync(geoJsonData);
                else await UpdateLocationsAsync(geoJsonData, locationValue, _isMapInitialized);
            }
            _isMapInitialized = true;
            StateHasChanged();
        }

        private async Task UpdateLocationsAsync(List<GeoJson> geoJsonData, string? locationValue = null, bool smoothTransition = true)
        {
            await MapBoxService.SetMarkersAsync(geoJsonData);
            if (!_isMapDragged && locationValue != null)
            {
                BoundingBox? destinationBoundingBox = await DestinationClient.GetDestinationBoundingBox(locationValue);
                if (destinationBoundingBox != null && destinationBoundingBox.HasValue)
                {
                    await MapBoxService.FitToBoundsAsync(new List<GeoJson> {
                        new GeoJson
                        {
                            Geometry = new Geometry
                            {
                                Coordinates = new List<double> { (double)destinationBoundingBox.MaxLongitude, (double)destinationBoundingBox.MaxLatitude},
                            }
                        },
                        new GeoJson
                        {
                            Geometry = new Geometry
                            {
                                Coordinates = new List<double> { (double)destinationBoundingBox.MinLongitude, (double)destinationBoundingBox.MinLatitude }
                            }
                        } 
                    }, smoothTransition);
                }
                else
                {
                    await MapBoxService.FitToBoundsAsync(geoJsonData, smoothTransition);
                }
            }
            _isMapDragged = false;
        }

        private async Task UpdateSingleLocationAsync(List<GeoJson> geoJsonData)
        {
            var coordinates = geoJsonData.Single().Geometry.Coordinates;
            await MapBoxService.SetZoomLevelAsync(16);
            await MapBoxService.SetCenterAsync((decimal)coordinates[0], (decimal)coordinates[1], false);
            await MapBoxService.SetCircleAsync(geoJsonData);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MapBoxService.MapFilter -= OnMapFilter;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
