using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class BedroomFilterModel : IFilterable
    {
        public int BedroomCount { get; set; }
        public FilterType Type { get; } = FilterType.Bedroom;

        public bool HasValue => !Equals(new BedroomFilterModel());
        public override bool Equals(object? obj) => obj != null && Equals((BedroomFilterModel)obj);

        private bool Equals(BedroomFilterModel bedroomFilterModel)
        {
            if (bedroomFilterModel == null) return false;

            return BedroomCount == bedroomFilterModel.BedroomCount;
        }

        public override int GetHashCode() => HashCode.Combine(BedroomCount);
    }
}
