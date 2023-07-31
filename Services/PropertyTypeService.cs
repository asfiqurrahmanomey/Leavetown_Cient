using Leavetown.Client.Clients.Contracts;
using Leavetown.Client.Constants;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Services
{
    public class PropertyTypeService : IPropertyTypeService
    {
        private ILocalStorageService _localStorageService;
        private IPropertyTypeClient _propertyTypeClient;

        public PropertyTypeService(ILocalStorageService localStorageService, IPropertyTypeClient propertyTypeClient)
        {
            _localStorageService = localStorageService;
            _propertyTypeClient = propertyTypeClient;
        }

        private async Task<List<PropertyTypeModel>> SetPropertyTypesAsync()
        {
            List<PropertyTypeModel> propertyTypes = (await _propertyTypeClient.GetPropertyTypeModelsAsync())?.ToList() ?? new();

            await _localStorageService.SetStorageValueAsync(StorageKey.PropertyTypes, propertyTypes);

            return propertyTypes;
        }

        public async Task<List<PropertyTypeModel>> GetPropertyTypesAsync()
        {
            List<PropertyTypeModel>? propertyTypes = await _localStorageService.GetStorageValueAsync<List<PropertyTypeModel>>(StorageKey.PropertyTypes);

            propertyTypes ??= await SetPropertyTypesAsync();

            return propertyTypes ?? new();
        }
    }
}
