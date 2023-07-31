using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class BedFilterModel: IFilterable
    {
        public int BedCount { get; set; }
        public FilterType Type { get; } = FilterType.Beds;

        public bool HasValue => !Equals(new BedFilterModel());
        public override bool Equals(object? obj) => obj != null && Equals((BedFilterModel)obj);

        private bool Equals(BedFilterModel bedsFilterModel)
        {
            if (bedsFilterModel == null) return false;

            return BedCount == bedsFilterModel.BedCount;
        }

        public override int GetHashCode() => HashCode.Combine(BedCount);
    }
}

