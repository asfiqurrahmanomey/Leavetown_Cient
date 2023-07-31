using System.Globalization;

namespace Leavetown.Client.Services.Contracts
{
    public interface ICultureService
    {
        Task<string> GetCultureAsync();
        Task SetCultureAsync(Uri uri);
    }
}
