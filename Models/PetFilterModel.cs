using Leavetown.Shared.Constants;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Models
{
    public class PetFilterModel : IFilterable
    {
        public FilterType Type { get; } = FilterType.Pets;
        public int PetCount { get; set; } = 0;

        public bool HasValue => !Equals(new PetFilterModel());

        public override bool Equals(object? obj) => obj != null && Equals((PetFilterModel)obj);

        private bool Equals(PetFilterModel petTypeAmount)
        {
            if (petTypeAmount == null) return false;

            return PetCount == petTypeAmount.PetCount;
        }

        public override int GetHashCode() => HashCode.Combine(PetCount);
    }
}
