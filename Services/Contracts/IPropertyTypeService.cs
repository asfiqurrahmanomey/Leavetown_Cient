using Leavetown.Shared.Models;

namespace Leavetown.Client.Services.Contracts
{
    public interface IPropertyTypeService
    {
        Task<List<PropertyTypeModel>> GetPropertyTypesAsync();
    }
}
