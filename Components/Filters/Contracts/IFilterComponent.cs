using Leavetown.Shared.Constants;

namespace Leavetown.Client.Components.Filters.Contracts
{
    public interface IFilterComponent
    {
        public void Reset(FilterType? filterType = null);

        public void Expand();
    }
}
