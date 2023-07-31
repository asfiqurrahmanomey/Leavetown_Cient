using Leavetown.Client.Constants;
using Leavetown.Client.Models;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.Contracts;

namespace Leavetown.Client.Components.Filters
{
    public static class FilterRepresentationHelper
    {
        public static string GetFilterTitle(FilterType? filterType, I18nText.SharedResources ResourcesShared)
        {
            string returnVal = string.Empty;

            if (filterType != null)
            {
                switch (filterType)
                {
                    case FilterType.Guests:
                        returnVal = ResourcesShared.Guests;
                        break;
                    case FilterType.Location:
                        returnVal = ResourcesShared.Location;
                        break;
                    case FilterType.Price:
                        returnVal = ResourcesShared.Price;
                        break;
                    case FilterType.Pets:
                        returnVal = ResourcesShared.Pets;
                        break;
                    case FilterType.Availability:
                        returnVal = ResourcesShared.Dates;
                        break;
                    case FilterType.SortBy:
                        returnVal = ResourcesShared.SortBy;
                        break;
                    case FilterType.Bedroom:
                        returnVal = ResourcesShared.NumberOfBedrooms;
                        break;
                    case FilterType.Bathroom:
                        returnVal = ResourcesShared.BathroomsTitle;
                        break;
                    case FilterType.Amenities:
                        returnVal = ResourcesShared.Amenities;
                        break;
                    case FilterType.Beds:
                        returnVal = ResourcesShared.Beds;
                        break;
                    case FilterType.PropertyType:
                        returnVal = ResourcesShared.PropertyTypes;
                        break;
                }
            }

            return returnVal;
        }

        public static string GetFilterRepresentation(IFilterable? filterModel, I18nText.SharedResources ResourcesShared)
        {
            string returnVal = string.Empty;

            if (filterModel != null)
            {
                switch (filterModel.Type)
                {
                    case FilterType.Guests:
                        GuestFilterModel guestFilterModel = (GuestFilterModel)filterModel;
                        returnVal = guestFilterModel.GuestCount == 0 ? ResourcesShared.Other : guestFilterModel.GuestCount > 1 ? string.Format(ResourcesShared.GuestPlural, guestFilterModel.GuestCount) : ResourcesShared.GuestSingular;
                        break;
                    case FilterType.Location:
                        LocationFilterModel locationFilterModel = (LocationFilterModel)filterModel;
                        returnVal = locationFilterModel.Name;
                        break;
                    case FilterType.Price:
                        PriceFilterModel priceFilterModel = (PriceFilterModel)filterModel;
                        returnVal = $"{priceFilterModel.Minimum} — {priceFilterModel.Maximum}{(priceFilterModel.Maximum == PriceFilterModel.AbsoluteMaximum ? "+" : "")}";
                        break;
                    case FilterType.Pets:
                        PetFilterModel petFilterModel = (PetFilterModel)filterModel;
                        returnVal = petFilterModel.PetCount == 0 ? string.Empty : petFilterModel.PetCount > 1 ? string.Format(ResourcesShared.PetPlural, petFilterModel.PetCount) : ResourcesShared.PetSingular;
                        break;
                    case FilterType.Availability:
                        AvailabilityFilterModel availabilityFilterModel = (AvailabilityFilterModel)filterModel;
                        returnVal = $"{availabilityFilterModel.Start:MMM dd}/{availabilityFilterModel.End:MMM dd}";
                        break;
                    case FilterType.SortBy:
                        SortingOptions sortingOptions = (SortingOptions)filterModel;
                        returnVal = GetSortingCriteriaRepresentation(sortingOptions.SortingCriteria, ResourcesShared);
                        break;
                    case FilterType.Bedroom:
                        BedroomFilterModel bedroomFilterModel = (BedroomFilterModel)filterModel;
                        returnVal = $"{bedroomFilterModel.BedroomCount}";
                        break;
                    case FilterType.Bathroom:
                        BathroomFilterModel bathroomFilterModel = (BathroomFilterModel)filterModel;
                        returnVal = $"{bathroomFilterModel.BathroomCount}";
                        break;
                    case FilterType.Beds:
                        BedFilterModel bedsFilterModel = (BedFilterModel)filterModel;
                        returnVal = $"{bedsFilterModel.BedCount}";
                        break;
                    case FilterType.Amenities:
                        AmenitiesFilterModel amenitiesFilterModel = (AmenitiesFilterModel)filterModel;
                        returnVal = $"{string.Join(", ", amenitiesFilterModel.Amenities?.Select(x => x.Name).ToList() ?? new())}";
                        break;
                    case FilterType.PropertyType:
                        PropertyTypeFilterModel propertyTypeFilterModel = (PropertyTypeFilterModel)filterModel;
                        returnVal = $"{string.Join(", ", propertyTypeFilterModel.PropertyTypes?.Select(x => x.Name).ToList() ?? new())}";
                        break;
                }
            }

            return returnVal;
        }

        public static string GetSortingCriteriaRepresentation(SortingCriteria option, I18nText.SharedResources ResourcesShared)
        {
            string returnVal;
            switch (option.SortField)
            {
                case (SortField.Price):
                    returnVal = string.Format("{0} {1}", ResourcesShared.SortOptionPrice, option.SortOrder == SortOrder.Ascending ? ResourcesShared.SortAscending : ResourcesShared.SortDescending);
                    break;
                case (SortField.Sleeps):
                    returnVal = string.Format("{0} {1}", ResourcesShared.SortOptionSleeps, option.SortOrder == SortOrder.Ascending ? ResourcesShared.SortAscending : ResourcesShared.SortDescending);
                    break;
                case (SortField.Relevance):
                    returnVal = ResourcesShared.SortOptionRelevance;
                    break;
                case (SortField.RecentlyAdded):
                    returnVal = ResourcesShared.SortOptionRecentlyAdded;
                    break;
                default:
                    returnVal = string.Empty;
                    break;
            }
            return returnVal;
        }

    }
}
