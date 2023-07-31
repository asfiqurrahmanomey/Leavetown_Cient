using Microsoft.AspNetCore.Components;
using BlazorDateRangePicker;
using Leavetown.Client.Components.Filters.Contracts;
using Leavetown.Client.I18nText;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;
using Leavetown.Shared.Models.ViewModels;
using Leavetown.Shared.Models.ViewModels.Contracts;

namespace Leavetown.Client.Components.Filters
{
    public partial class ListingAvailabilityFilter : IDisposable, IAvailabilityFilter
    {
        [Parameter] public string Id { get; set; } = AvailabilityFilter.IdDefault;
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public bool ClearTitleOnSelect { get; set; }
        [Parameter] public bool DisablePreviousDates { get; set; }
        [Parameter] public bool Inline { get; set; }
        [Parameter] public bool ShowOnlyOneCalendar { get; set; }
        [Parameter] public bool LinkedCalendars { get; set; }
        [Parameter] public bool UpdateUrlOnChange { get; set; }
        [Parameter] public bool UseCheckinCheckout { get; set; }
        [Parameter] public AvailabilityFilterModel AvailabilityValue { get; set; } = new();
        [Parameter] public DateTimeOffset? MaxDateOverride {
            get {
                return _calculatedMaxDateOverride.HasValue 
                    ? _calculatedMaxDateOverride 
                    : _defaultMaxDateOverride;
            }
            set {
                    _defaultMaxDateOverride = value;
                }
        }
        [Parameter] public IListingViewModel Listing { get; set; } = new ListingViewModel();
        [Parameter] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
        [CascadingParameter] public SharedResources ResourcesShared { get; set; } = default!;

        [Parameter] public EventCallback<AvailabilityFilterModel> AvailabilityValueChanged { get; set; } = default;
        [Parameter] public EventCallback<DateTimeOffset> StartDateSelected { get; set; } = default;
        [Parameter] public Action<AvailabilityFilterModel> Changed { get; set; } = default!;
        [Parameter] public Action<AvailabilityFilter.DateEnablingEventArgs> DateEnabling { get; set; } = default!;
        [Parameter] public Action<AvailabilityFilter.DateStylingEventArgs> DateStyling { get; set; } = default!;

        private AvailabilityFilter? _availabilityFilter = new();

        public Popover? DatesPopover => _availabilityFilter?.DatesPopover;
        public DateRangePicker? DateRangePicker => _availabilityFilter?.DateRangePicker;
        public bool IsEndDateSelecting => _availabilityFilter?.IsEndDateSelecting ?? false;

        private Dictionary<DateTime, PricingAvailabilityModel>? _pricingAvailabilities;
        private PricingAvailabilityModel? _startPricingAvailability;
        private DateTimeOffset? _defaultMaxDateOverride;
        private DateTimeOffset? _calculatedMaxDateOverride;

        private bool _disposed;

        protected override void OnParametersSet()
        {
            _pricingAvailabilities = Listing.PricingAvailabilities
                ?.Where(x => x != null
                                && x.Date.HasValue)
                .OrderBy(x => x!.Date!.Value.Date)
                .GroupBy(x => x!.Date!.Value.Date)
                .ToDictionary(x => x.Key, x => x.First()!)
                ?? new Dictionary<DateTime, PricingAvailabilityModel>();
        }

        private void OnStartDateSelected(DateTimeOffset offset)
        {
            _startPricingAvailability = null;
            _pricingAvailabilities?.TryGetValue(offset.Date, out _startPricingAvailability);

            var maxEndDate = _pricingAvailabilities?.FirstOrDefault(kvp => (kvp.Key > offset.Date) && !kvp.Value.IsAvailable).Value.Date;
            if (maxEndDate != null) _calculatedMaxDateOverride = (DateTimeOffset)maxEndDate;

            StartDateSelected.InvokeAsync(offset);
            StateHasChanged();
        }

        private void OnChanged(AvailabilityFilterModel availabilityFilterModel)
        {
            _startPricingAvailability = null;
            _calculatedMaxDateOverride = null;
            
            if (availabilityFilterModel.HasValue)
                Changed?.Invoke(availabilityFilterModel);
        }

        private void OnDateEnabling(AvailabilityFilter.DateEnablingEventArgs dateEnablingEventArgs)
        {
            if (dateEnablingEventArgs.Enabled)
            {
                dateEnablingEventArgs.Enabled = AssertDateIsAvailable(dateEnablingEventArgs.Offset.Date, out _);
            }

            DateEnabling?.Invoke(dateEnablingEventArgs);
        }

        private void OnDateStyling(AvailabilityFilter.DateStylingEventArgs dateStylingEventArgs)
        {
            if (string.IsNullOrWhiteSpace(dateStylingEventArgs.CssClass))
            {
                DateTime date = dateStylingEventArgs.Offset.Date.Date;

                if (!AssertDateIsAvailable(date, out bool isCheckOutRestriction)
                    && !isCheckOutRestriction)
                {
                    dateStylingEventArgs.CssClass = AvailabilityFilter.DateUnavailableCssClass;
                }
            }

            DateStyling?.Invoke(dateStylingEventArgs);
        }

        public void Expand() => _availabilityFilter?.Expand();
        public void Reset(FilterType? filterType = null) => _availabilityFilter?.Reset(filterType);
        public void Set(AvailabilityFilterModel? availability) => _availabilityFilter?.Set(availability);

        private bool AssertDateIsAvailable(DateTime date, out bool isCheckOutRestricted)
        {
            isCheckOutRestricted = false;

            // Selecting end date and date is less than selected start date
            if (IsEndDateSelecting
                && DateRangePicker != null
                && DateRangePicker.TStartDate.HasValue
                && DateRangePicker.TStartDate.Value > date) return false;

            // No Listing Pricing Availabilities available
            if (_pricingAvailabilities == null) return true;

            // Not able to find pricing availability for date
            if (!_pricingAvailabilities.TryGetValue(date, out PricingAvailabilityModel? pricingAvailability)) return false;

            if (IsEndDateSelecting)
            {
                // test for Check Out ONLY
                // back office says date is not available, but can we check out?

                bool isCheckOutOnlyDateInterpreted = false;
                if (!pricingAvailability.IsAvailable && pricingAvailability.CheckOutAllowed)
                {
                    if (!_pricingAvailabilities.TryGetValue(date.AddDays(-1), out PricingAvailabilityModel? pricingAvailabilityPreviousDay)) return false;

                    // even though today is marked as CheckoutAllowed, if the previous day was not available
                    // then that date cannot be a part of our date range
                    if (!pricingAvailabilityPreviousDay.IsAvailable) return false;
                    isCheckOutOnlyDateInterpreted = true;
                }

                if (_calculatedMaxDateOverride != null)
                {
                    if (!isCheckOutOnlyDateInterpreted && (pricingAvailability.Date >= _calculatedMaxDateOverride)) return false;
                }

                if (!pricingAvailability.CheckOutAllowed // Checkout is explicitly not allowed
                    || _startPricingAvailability == null
                    || (decimal)(pricingAvailability.Date!.Value - _startPricingAvailability.Date!.Value).TotalDays < _startPricingAvailability.MinStayNight) // Insufficient nights stayed
                {
                    isCheckOutRestricted = true;
                    return false;
                }
            }
            else
            {
                if (!pricingAvailability.IsAvailable) return false; // No master availability
                if (!pricingAvailability.CheckInAllowed) return false; // Checkin explicitly not allowed
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _availabilityFilter?.Dispose();
                }

                _disposed = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
