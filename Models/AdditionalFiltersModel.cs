namespace Leavetown.Client.Models
{
    public class AdditionalFiltersModel
    {
        public BedroomFilterModel BedroomFilterModel { get; set; } = default!;
        public BedFilterModel BedsFilterModel { get; set; } = default!;
        public BathroomFilterModel BathroomFilterModel { get; set; } = default!;
        public PropertyTypeFilterModel PropertyTypeFilterModel { get; set; } = default!;
        public AmenitiesFilterModel AmenitiesFilterModel { get; set; } = default!;
    }
}
