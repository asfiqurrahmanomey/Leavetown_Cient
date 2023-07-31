
namespace Leavetown.Client.Models
{
    public enum SortField
    {
        RecentlyAdded,
        Relevance,
        Price,
        Sleeps
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public partial class SortingCriteria
    {
        public SortingCriteria()
        {
        }

        public SortingCriteria(SortField? sortField, SortOrder? sortOrder, string? tag, string sortQuery)
        {
            SortField = sortField;
            SortOrder = sortOrder;
            Tag = tag;
            SortQuery = sortQuery ?? throw new ArgumentNullException(nameof(sortQuery));
        }

        public SortField? SortField { get; }
        public SortOrder? SortOrder{ get; }
        public string? Tag { get; set; } = string.Empty;
        public string SortQuery { get; set; } = default!;
    }
}
