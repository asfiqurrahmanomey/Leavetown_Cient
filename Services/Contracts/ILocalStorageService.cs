using Leavetown.Client.Models.Events;

namespace Leavetown.Client.Services.Contracts
{
    public interface ILocalStorageService
    {
        event EventHandler<LocalStorageChangedEventArgs> LocalStorageChanged;
        Task<T> SetStorageValueAsync<T>(string key, T value) where T : class;
        Task<T?> GetStorageValueAsync<T>(string key) where T : class;
    }
}
