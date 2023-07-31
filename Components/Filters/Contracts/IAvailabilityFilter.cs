using Microsoft.AspNetCore.Components;
using BlazorDateRangePicker;
using Leavetown.Client.Constants;
using Leavetown.Client.I18nText;
using Leavetown.Shared.Models;

namespace Leavetown.Client.Components.Filters.Contracts
{
    public interface IAvailabilityFilter : IFilterComponent
    {
        string Id { get; set; }
        string Title { get; set; }
        bool ClearTitleOnSelect { get; set; }
        bool DisablePreviousDates { get; set; }
        bool Inline { get; set; }
        bool ShowOnlyOneCalendar { get; set; }        
        bool UpdateUrlOnChange { get; set; }
        bool UseCheckinCheckout { get; set; }
        AvailabilityFilterModel AvailabilityValue { get; set; }
        Dictionary<string, object> CapturedAttributes { get; set; }
        SharedResources ResourcesShared { get; set; }

        EventCallback<AvailabilityFilterModel> AvailabilityValueChanged { get; set; }
        EventCallback<DateTimeOffset> StartDateSelected { get; set; }
        Action<AvailabilityFilterModel> Changed { get; set; }
        Action<AvailabilityFilter.DateEnablingEventArgs> DateEnabling { get; set; }
        Action<AvailabilityFilter.DateStylingEventArgs> DateStyling { get; set; }

        Popover? DatesPopover { get; }
        DateRangePicker? DateRangePicker { get; }
        bool IsEndDateSelecting { get; }
        DateTimeOffset? MaxDateOverride { get; set; }

        void Set(AvailabilityFilterModel? availability);
    }
}