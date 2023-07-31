using Leavetown.Shared.Models;

namespace Leavetown.Client.Clients.Contracts
{
    public interface IPropertyTypeClient
    {
        Task<IEnumerable<PropertyTypeModel>?> GetPropertyTypeModelsAsync();
    }
}
