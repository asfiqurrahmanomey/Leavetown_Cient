using Leavetown.Client.Models;
using Leavetown.Client.Models.MapBox;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace Leavetown.Client.Utilities.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static string GetQuery(this NavigationManager navigationManager) => new Uri(navigationManager.Uri).Query;

        public static Uri AddQueryParameter(this NavigationManager navigationManager, AvailabilityFilterModel? availability)
        {
            if (availability == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            string uri = navigationManager.GetUriWithQueryParameter(nameof(FilterType.Availability).ToLower(), value: availability.HasValue ?
                $"{availability.Start:yyyy-MM-dd}~{availability.End:yyyy-MM-dd}" : null);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, DateTime? checkInDate, DateTime? checkoutDate)
        {
            if (checkInDate == null || checkoutDate == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            Dictionary<string, object?> parameters = new()
            {
                { nameof(FilterType.CheckIn).ToSnakeCase(), checkInDate != DateTime.MinValue ? $"{checkInDate:yyyy-MM-dd}" : null },
                { nameof(FilterType.CheckOut).ToSnakeCase(), checkoutDate != DateTime.MinValue ? $"{checkoutDate:yyyy-MM-dd}" : null },
            };

            string uri = navigationManager.GetUriWithQueryParameters(parameters);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, LocationFilterModel? location)
        {
            if (location == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            string uri = navigationManager.GetUriWithQueryParameter(nameof(FilterType.Location).ToLower(), value: location.HasValue ? location.Value : null);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, BoundingBox? boundingBox)
        {
            if (boundingBox == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            string uri = navigationManager.GetUriWithQueryParameter(nameof(FilterType.Location).ToLower(), value: boundingBox.HasValue ? $"map+view~{boundingBox}" : null);

            navigationManager.NavigateTo(uri);
            return new Uri(uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, PriceFilterModel? priceRange)
        {
            if (priceRange == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            string uri = navigationManager.GetUriWithQueryParameter(nameof(FilterType.Price).ToLower(), value: priceRange.HasValue ? $"{priceRange.Minimum}~{priceRange.Maximum}" : null);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, GuestFilterModel? guestTypeAmount)
        {
            if (guestTypeAmount == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            Dictionary<string, object?> parameters = new()
            {
                { nameof(FilterType.Guests).ToLower(), guestTypeAmount.HasValue && guestTypeAmount.GuestCount > 0 ? guestTypeAmount.GuestCount.ToString() : null },
                { nameof(FilterType.Adults).ToLower(), guestTypeAmount.HasValue && guestTypeAmount.AdultCount > 0 ? guestTypeAmount.AdultCount.ToString() : null },
                { nameof(FilterType.Children).ToLower(), guestTypeAmount.HasValue && guestTypeAmount.ChildCount > 0 ? guestTypeAmount.ChildCount.ToString() : null }
            };

            string uri = navigationManager.GetUriWithQueryParameters(parameters);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, AdditionalFiltersModel additionalFiltersModel)
        {
            if (additionalFiltersModel == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            AmenityModel[]? amenities = additionalFiltersModel?.AmenitiesFilterModel?.Amenities?.ToArray();
            PropertyTypeModel[]? propertyTypes = additionalFiltersModel?.PropertyTypeFilterModel?.PropertyTypes?.ToArray();
            BedroomFilterModel? bedrooms = additionalFiltersModel?.BedroomFilterModel;
            BathroomFilterModel? bathrooms = additionalFiltersModel?.BathroomFilterModel;
            BedFilterModel? beds = additionalFiltersModel?.BedsFilterModel;

            Dictionary<string, object?> parameters = new()
            { 
                { nameof (FilterType.Amenities).ToLower(), amenities != null && amenities.Length > 0 ? string.Join('|', amenities.Select(a => HttpUtility.UrlEncode(a.Name))) : null },
                { nameof (FilterType.PropertyType).ToSnakeCase(), propertyTypes != null && propertyTypes.Length > 0 ? string.Join('|', propertyTypes.Select(x => x.Name)) : null },
                { nameof(FilterType.Bedroom).ToLower(), bedrooms != null && bedrooms.BedroomCount > 0 ? bedrooms.BedroomCount.ToString() : null },
                { nameof(FilterType.Bathroom).ToLower(), bathrooms != null && bathrooms.BathroomCount > 0 ? bathrooms.BathroomCount.ToString() : null },
                { nameof(FilterType.Beds).ToLower(), beds != null && beds.BedCount > 0 ? beds.BedCount.ToString() : null }
            };

            string uri = navigationManager.GetUriWithQueryParameters(parameters);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, PetFilterModel? petTypeAmount)
        {
            if (petTypeAmount == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            string uri = navigationManager.GetUriWithQueryParameter(nameof(FilterType.Pets).ToLower(), value: petTypeAmount.HasValue ? petTypeAmount.PetCount : null);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, SortingCriteria sortingCriteria)
        {
            if (sortingCriteria == null) return NavigateToDecodedUri(navigationManager, navigationManager.Uri);

            string uri = navigationManager.GetUriWithQueryParameter(nameof(FilterType.SortBy).ToSnakeCase(), value: sortingCriteria.Tag);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri AddQueryParameter(this NavigationManager navigationManager, string name, string value)
        {
            string uri = navigationManager.GetUriWithQueryParameter(name, value);
            return NavigateToDecodedUri(navigationManager, uri);
        }

        public static Uri NavigateToDecodedUri(this NavigationManager navigationManager, string uri)
        {
            string decodedUri = HttpUtility.UrlDecode(uri);
            if (!string.Equals(uri, navigationManager.Uri)) navigationManager.NavigateTo(decodedUri);
            return new Uri(decodedUri);
        }

        public static Uri RemoveQueryStringByKey(this NavigationManager navigationManager, string key)
        {
            var uri = new Uri(navigationManager.Uri);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            string result = newQueryString.Count > 0 ? $"{pagePathWithoutQueryString}?{newQueryString}" : pagePathWithoutQueryString;
            navigationManager.NavigateTo(result);

            return new Uri(result);
        }
    }
}
