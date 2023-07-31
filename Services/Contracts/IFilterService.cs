using Leavetown.Shared.Models.Contracts;
using Microsoft.Extensions.Primitives;

namespace Leavetown.Client.Services.Contracts
{
    public interface IFilterService<T> where T : IFilterable
    {
        T Parse(KeyValuePair<string, StringValues> query);
        string GetFilterQuery(T filter);
    }
}
