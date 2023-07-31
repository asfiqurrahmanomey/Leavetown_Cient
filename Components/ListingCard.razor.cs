using Leavetown.Client.Components.Inline;
using Leavetown.Client.Models.ViewModels;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Radzen;

namespace Leavetown.Client.Components
{
    public partial class ListingCard
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IAvailabilityFilterService AvailabilityFilterService { get; set; } = default!;
        [Inject] private TooltipService TooltipService { get; set; } = default!;

        [Parameter]
        public ListingCardViewModel? Listing
        {
            get => _listingCardViewModel;
            set
            {
                _listingCardViewModel = value;
            }
        }
        [Parameter] public bool IsResponsive { get; set; } = true;
        [Parameter] public string Tag { get; set; } = "";
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;


        [Parameter]
        public string Culture
        {
            get => _culture;
            set => _culture = value;
        }

        private string _culture = "";
        private ListingCardViewModel? _listingCardViewModel;
        private ListingViewModel? _listing => _listingCardViewModel?.Listing;
        private ElementReference _priceElement;
        private PriceInfo? _priceInfo;

        private ListingCardViewModel? _prevListingCardViewModel;
        private bool _shouldRender;

        protected override async Task OnParametersSetAsync()
        {
            _shouldRender = false;
            if (string.IsNullOrEmpty(Culture))
            {
                Culture = await CultureService.GetCultureAsync();
                _shouldRender = true;
            }
            if (Listing != null && !Listing.Equals(_prevListingCardViewModel))
            {
                _prevListingCardViewModel = Listing;

                _shouldRender = true;
            }

            await base.OnParametersSetAsync();
        }

        protected override bool ShouldRender() => _shouldRender;

        public string SearchFilterFromQueryString
        {
            get {
                var query = QueryHelpers.ParseQuery(new Uri(NavigationManager.Uri).Query);
                if (query.TryGetValue(nameof(FilterType.Availability).ToLower(), out var availability))
                {
                    AvailabilityFilterModel filter = AvailabilityFilterService.Parse(
                        new KeyValuePair<string, StringValues>(nameof(FilterType.Availability).ToLower(),
                        availability));

                    Dictionary<string, object?> parameters = new()
                    {
                        {
                            nameof(FilterType.CheckIn).ToSnakeCase(),
                            $"{filter.Start.Date:yyyy-MM-dd}"
                        },
                        {
                            nameof(FilterType.CheckOut).ToSnakeCase(),
                            $"{filter.End.Date:yyyy-MM-dd}"
                        }
                    };
                    string uri = NavigationManager.GetUriWithQueryParameters(parameters);
                    return new Uri(uri).Query;
                }

                return new Uri(NavigationManager.Uri).Query;
            }
        }

        private string GetBedroomsRepresentation()
        {
            string formatString = (_listingCardViewModel?.Listing.NumberOfBedrooms) switch
            {
                0 => ResourcesShared.BedroomNone,
                1 => ResourcesShared.BedroomSingular,
                _ => ResourcesShared.BedroomPlural,
            };
            return string.Format(formatString, _listingCardViewModel?.Listing.NumberOfBedrooms).ToUpper();
        }

        void ShowTooltip(ElementReference elementReference)
        {
            if (_priceInfo != null && 
                Listing != null && 
                Listing.Listing.PricingAvailabilities != null &&
                !Listing.Listing.PricingAvailabilities.All(x => x != null && !x.IsAvailable) && 
                Listing.Listing.PricingAvailabilities.Count() != 0)
            {
                TooltipService.Open(elementReference, ResourcesShared.PricingTooltip, new TooltipOptions()
                {
                    Position = TooltipPosition.Top, // set to Top because there's a bug with bottom
                    Style = "background-color: var(--colors-text); width: 20rem; white-space: normal"
                });
            }
        }
    }
}
