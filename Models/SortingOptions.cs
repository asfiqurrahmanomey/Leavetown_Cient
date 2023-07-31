using Leavetown.Client.Constants;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class SortingOptions : IFilterable
    {
        public FilterType Type => FilterType.SortBy;

        public SortingCriteria SortingCriteria { get; set; } = Default;

        public static SortingCriteria Default { get; } = new SortingCriteria();

        public static SortingCriteria Relevance { get; } = new SortingCriteria
        (
            SortField.Relevance,
            null,
            null,
            "id"
        );

        public static SortingCriteria PriceLowToHigh { get; } = new SortingCriteria
        (
            SortField.Price,
            SortOrder.Ascending,
            "nightly_price_asc",
            "average_price"
        );

        public static SortingCriteria PriceHighToLow { get; } = new SortingCriteria
        (
            SortField.Price,
            SortOrder.Descending,
            "nightly_price_desc",
            "average_price DESC"
        );

        public static SortingCriteria SleepsLowToHigh { get; } = new SortingCriteria
        (
            SortField.Sleeps,
            SortOrder.Ascending,
            "sleeps_asc",
            "number_of_guests"
        );

        public static SortingCriteria SleepsHighToLow { get; } = new SortingCriteria
        (
            SortField.Sleeps,
            SortOrder.Descending,
            "sleeps_desc",
            "number_of_guests DESC"
        );

        public static SortingCriteria RecentlyAdded { get; } = new SortingCriteria
        (
            SortField.RecentlyAdded,
            null,
            "latest",
            "date_added"
        );

        public bool HasValue => !Equals(new SortingOptions());

        private bool Equals(SortingOptions sortingOptions)
        {
            if (sortingOptions == null) return false;

            return sortingOptions.SortingCriteria.SortField.Equals(SortingCriteria.SortField);
        }
    }
}
