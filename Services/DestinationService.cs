using Leavetown.Client.Clients;
using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services
{
    public class DestinationService : IDestinationService
    {
        private ILocalStorageService _localStorageService;
        private IDestinationClient _destinationClient;

        public DestinationService(ILocalStorageService localStorageService, IDestinationClient destinationClient)
        {
            _localStorageService = localStorageService;
            _destinationClient = destinationClient;
        }

        private async Task<List<DestinationModel>> SetDestinationsAsync()
        {
            List<DestinationModel> destinations = (await _destinationClient.GetDestinationModelsAsync())?.ToList() ?? new();

            await _localStorageService.SetStorageValueAsync(StorageKey.Destinations, destinations);

            return destinations;
        }

        public async Task<List<DestinationModel>> GetDestinationsAsync()
        {
            List<DestinationModel>? destinations = await _localStorageService.GetStorageValueAsync<List<DestinationModel>>(StorageKey.Destinations);

            destinations ??= await SetDestinationsAsync();

            return destinations ?? new();
        }
    }
}
