using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class BathroomFilterModel: IFilterable
    {
        public int BathroomCount { get; set; }
        public FilterType Type { get; } = FilterType.Bathroom;

        public bool HasValue => !Equals(new BathroomFilterModel());
        public override bool Equals(object? obj) => obj != null && Equals((BathroomFilterModel)obj);

        private bool Equals(BathroomFilterModel bathroomFilterModel)
        {
            if (bathroomFilterModel == null) return false;

            return BathroomCount == bathroomFilterModel.BathroomCount;
        }

        public override int GetHashCode() => HashCode.Combine(BathroomCount);
    }
}

