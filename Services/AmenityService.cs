using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services
{
    public class AmenityService : IAmenityService
    {
        private ILocalStorageService _localStorageService;
        private IAmenityClient _amenityClient;

        public AmenityService(ILocalStorageService localStorageService, IAmenityClient amenityClient)
        {
            _localStorageService = localStorageService;
            _amenityClient = amenityClient;
        }

        private async Task<List<AmenityModel>> SetAmenitiesAsync()
        {
            List<AmenityModel> amenities = (await _amenityClient.GetAmenityModelsAsync())?.ToList() ?? new();

            await _localStorageService.SetStorageValueAsync(StorageKey.Amenities, amenities);

            return amenities;
        }

        public async Task<List<AmenityModel>> GetAmenitiesAsync()
        {
            List<AmenityModel>? amenities = await _localStorageService.GetStorageValueAsync<List<AmenityModel>>(StorageKey.Amenities);

            amenities ??= await SetAmenitiesAsync();

            return amenities ?? new();
        }
    }
}
