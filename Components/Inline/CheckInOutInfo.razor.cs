using Microsoft.AspNetCore.Components;

namespace Leavetown.Client.Components.Inline
{
    public partial class CheckInOutInfo
    {
        [Parameter, EditorRequired] public DateTime Date { get; set; } = default!;
        [Parameter, EditorRequired] public TimeSpan? Time { get; set; } = default!;
        [Parameter, EditorRequired] public bool IsCheckIn { get; set; } = default!;
        [CascadingParameter] public I18nText.MicroSiteSpecificResources ResourcesMicroSiteSpecific { get; set; } = default!;
        [CascadingParameter] public I18nText.SharedResources ResourcesShared { get; set; } = default!;

        private DateTime _dateTime;

        protected override void OnInitialized()
        {
            _dateTime = new DateTime(
                year: Date.Year,
                month: Date.Month,
                day: Date.Day,
                hour: Time.HasValue ? Time.Value.Hours : Date.Hour,
                minute: Time.HasValue ? Time.Value.Minutes : Date.Minute,
                second: Time.HasValue ? Time.Value.Seconds : Date.Second
            );
        }
    }
}
