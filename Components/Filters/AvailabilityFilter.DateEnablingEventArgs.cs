namespace Leavetown.Client.Components.Filters
{
    public partial class AvailabilityFilter
    {
        public class DateEnablingEventArgs
        {
            public DateTimeOffset Offset { get; }
            public bool Enabled { get; set; }

            public DateEnablingEventArgs(DateTimeOffset offset)
            {
                Offset = offset;
            }
        }
    }
}
