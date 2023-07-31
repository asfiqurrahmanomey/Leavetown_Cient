using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class PriceFilterModel : IFilterable
    {
        public FilterType Type { get; } = FilterType.Price;
        public int Minimum { get; set; } = 0;
        public int Maximum { get; set; } = 0;
        public static int AbsoluteMaximum => 999;
        public override bool Equals(object? obj) => obj != null && Equals((PriceFilterModel)obj);

        private bool Equals(PriceFilterModel priceRange)
        {
            if (priceRange == null) return false;

            return Minimum == priceRange.Minimum && Maximum == priceRange.Maximum;
        }

        public bool HasValue => !Equals(new PriceFilterModel());

        public override int GetHashCode() => HashCode.Combine(Minimum, Maximum);
    }
}
