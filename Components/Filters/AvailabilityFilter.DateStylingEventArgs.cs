namespace Leavetown.Client.Components.Filters
{
    public partial class AvailabilityFilter
    {
        public class DateStylingEventArgs
        {
            public DateTimeOffset Offset { get; }
            public string CssClass { get; set; } = string.Empty;

            public DateStylingEventArgs(DateTimeOffset offset)
            {
                Offset = offset;
            }
        }
    }
}
