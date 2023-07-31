using Leavetown.Client.Models;
using Leavetown.Client.Models.Events;
using Leavetown.Client.Services.Contracts;
using Microsoft.JSInterop;
using Serilog;
using System.Text.Json;

namespace Leavetown.Client.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        private IJSRuntime _jsRuntime;
        private int _expiryInterval = 60000 * 60 * 24; // 1 day in milliseconds

        public event EventHandler<LocalStorageChangedEventArgs> LocalStorageChanged = default!;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T> SetStorageValueAsync<T>(string key, T value) where T : class
        {
            string serializedValue = JsonSerializer.Serialize(new CacheItem<T> { Data = value, ExpiryTimeStamp = DateTime.Now });
            if (LocalStorageChanged != null) LocalStorageChanged.Invoke(this, new LocalStorageChangedEventArgs(key, serializedValue));

            return await _jsRuntime.InvokeAsync<T>("setLocalStorage", new[] { key, serializedValue });
        }

        public async Task<T?> GetStorageValueAsync<T>(string key) where T : class
        {
            string? storageItem = await _jsRuntime.InvokeAsync<string>("getLocalStorage", key);
            if(storageItem == null) return default;
            try
            {
                CacheItem<T>? cacheItem = JsonSerializer.Deserialize<CacheItem<T>>(storageItem);
                return !ValidateCache(cacheItem) ? default : cacheItem?.Data;
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Error with retrieving local storage value with key '{Key}': {Message}", key, ex.Message);
                return default;
            }
        }

        private bool ValidateCache<T>(CacheItem<T>? cacheItem) where T : class =>
            cacheItem != null &&
            cacheItem.ExpiryTimeStamp > DateTime.Now.AddMilliseconds(-_expiryInterval);
    }
}
