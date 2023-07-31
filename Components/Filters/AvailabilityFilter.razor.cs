using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using BlazorDateRangePicker;
using Leavetown.Client.Components.Filters.Contracts;
using Leavetown.Client.Services.Contracts;
using Leavetown.Client.Utilities.Extensions;
using Leavetown.Shared.Constants;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Components.Filters
{
    public partial class AvailabilityFilter : IDisposable, IAvailabilityFilter
    {
        public const string IdDefault = "dates-popover";
        public const string DateUnavailableCssClass = "unavailable";

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IAvailabilityFilterService AvailabilityFilterService { get; set; } = default!;

        [Parameter] public string Id { get; set; } = IdDefault;
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public bool ClearTitleOnSelect { get; set; } = false;
        [Parameter] public bool DisablePreviousDates { get; set; } = false;
        [Parameter] public bool Inline { get; set; }
        [Parameter] public bool ShowOnlyOneCalendar { get; set; } = false;
        [Parameter] public bool LinkedCalendars { get; set; } = true;
        [Parameter] public bool UpdateUrlOnChange { get; set; } = true;
        [Parameter] public bool UseCheckinCheckout { get; set; } = false;
        [Parameter] public AvailabilityFilterModel AvailabilityValue { get; set; } = new();
        [Parameter] public DateTimeOffset? MaxDateOverride { get; set; }
        [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object> CapturedAttributes { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        [Parameter] public EventCallback<AvailabilityFilterModel> AvailabilityValueChanged { get; set; } = default;
        [Parameter] public EventCallback<DateTimeOffset> StartDateSelected { get; set; } = default;
        [Parameter] public Action<AvailabilityFilterModel> Changed { get; set; } = default!;
        [Parameter] public Action<DateEnablingEventArgs> DateEnabling { get; set; } = default!;
        [Parameter] public Action<DateStylingEventArgs> DateStyling { get; set; } = default!;

        public bool IsEndDateSelecting { get; private set; } = false;
        
        public Popover? DatesPopover { get; private set; } = new();
        public DateRangePicker? DateRangePicker { get; private set; } = new();
        
        private bool _disposedValue;

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            Title = ResourcesShared.DateFilterTitlePlaceholder;
        }

        private string FilterTitle
        {
            get => FilterRepresentationHelper.GetFilterRepresentation(AvailabilityValue, ResourcesShared);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateFromUrlAsync(NavigationManager.Uri);
                StateHasChanged();
            }
        }

        private async Task UpdateFromUrlAsync(string uri)
        {
            if (uri == null) return;

            Dictionary<string, StringValues>? queryValues = QueryHelpers.ParseQuery(new Uri(uri).Query);
            AvailabilityFilterModel availability;
            if (UseCheckinCheckout)
            {
                queryValues.TryGetValue(nameof(FilterType.CheckIn).ToSnakeCase(), out var checkInValue);
                queryValues.TryGetValue(nameof(FilterType.CheckOut).ToSnakeCase(), out var checkOutValue);
                availability = checkInValue != StringValues.Empty && checkOutValue != StringValues.Empty ?
                    AvailabilityFilterService.Parse(checkInValue, checkOutValue) :
                    new AvailabilityFilterModel();
            }
            else
            {
                queryValues.TryGetValue(nameof(FilterType.Availability).ToLower(), out var value);
                availability = value != StringValues.Empty ?
                    AvailabilityFilterService.Parse(new KeyValuePair<string, StringValues>(nameof(FilterType.Availability).ToLower(), value)) :
                    new AvailabilityFilterModel();
            }

            if (!availability.Equals(AvailabilityValue))
            {
                AvailabilityValue = availability;
                Set(availability.HasValue ? availability : null);
                Changed?.Invoke(AvailabilityValue);
                await AvailabilityValueChanged.InvokeAsync(AvailabilityValue);
                StateHasChanged();
            }           
        }

        public void Reset(FilterType? filterType = null)
        {
            IsEndDateSelecting = false;
            AvailabilityValue = new AvailabilityFilterModel();
            DateRangePicker!.TStartDate = null;
            DateRangePicker.TEndDate = null;
            StateHasChanged();
            OnDateValueChanged();
        }

        public void Expand() => DatesPopover!.ToggleCardVisibility();

        public void Set(AvailabilityFilterModel? availability)
        {
            DateRangePicker!.TStartDate = availability?.Start.Date;
            DateRangePicker.TEndDate = availability?.End.Date;
            
            AvailabilityValue.Start = availability?.Start ?? DateTime.MinValue;
            AvailabilityValue.End = availability?.End ?? DateTime.MinValue;            
        }

        private void OnStartDateSelected(DateTimeOffset date)
        {
            IsEndDateSelecting = true;
            StartDateSelected.InvokeAsync(date);
            StateHasChanged();
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => InvokeAsync(async () => await UpdateFromUrlAsync(e.Location));

        private void OnDateValueChanged()
        {
            IsEndDateSelecting = false;

            AvailabilityValue.Start = DateRangePicker!.TStartDate.HasValue ? DateRangePicker.TStartDate.Value.Date : DateTime.MinValue;
            AvailabilityValue.End = DateRangePicker.TEndDate.HasValue ? DateRangePicker.TEndDate.Value.Date : DateTime.MinValue;

            Changed?.Invoke(AvailabilityValue);
            AvailabilityValueChanged.InvokeAsync(AvailabilityValue);

            if (!UpdateUrlOnChange) return;

            if (UseCheckinCheckout)
            {
                NavigationManager.AddQueryParameter(AvailabilityValue.Start, AvailabilityValue.End);
            }
            else
            {
                NavigationManager.AddQueryParameter(AvailabilityValue);
            }
        }

        private bool OnDateEnabling(DateTimeOffset offset)
        {
            var args = new DateEnablingEventArgs(offset)
            {
                Enabled = AssertDateIsAvailable(offset.Date)
            };

            DateEnabling?.Invoke(args);
            return args.Enabled;
        }

        private DateTimeOffset? GetMinDate()
        {
            return DisablePreviousDates ? DateTime.Today : (DateTimeOffset?)null;
        }

        private DateTimeOffset? GetMaxDate()
        {
            DateTimeOffset? returnVal = (DateTimeOffset?)null;
            if (MaxDateOverride != null) returnVal = MaxDateOverride;
            return returnVal;
        }

        private object OnDateStyling(DateTimeOffset offset)
        {
            var args = new DateStylingEventArgs(offset);

            DateTime date = offset.Date.Date;

            if (!AssertDateIsAvailable(date)
                && (!DateRangePicker!.TStartDate.HasValue
                    || DateRangePicker.TStartDate.Value != date))
            {
                args.CssClass = DateUnavailableCssClass;
            };

            DateStyling?.Invoke(args);
            return args.CssClass;
        }

        private bool AssertDateIsAvailable(DateTime date)
        {
            // Date is in the past
            if (date < DateTime.Today) return false;

            // Selecting end date and date is equal to selected start date
            if (DateRangePicker!.TStartDate.HasValue
                && DateRangePicker.TStartDate.Value == date) return false;

            return true;
        }

        private string GetTitle()
        {
            return AvailabilityValue.HasValue ? $"{ResourcesShared.Dates}: {FilterTitle}" : ResourcesShared.Dates;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    NavigationManager.LocationChanged -= OnLocationChanged;
                }

                _disposedValue = true;
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
