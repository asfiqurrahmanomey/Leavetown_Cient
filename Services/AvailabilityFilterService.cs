using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Models;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class AvailabilityFilterService : IAvailabilityFilterService
    {
        public AvailabilityFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            var dates = query.Value.ToString().Split('~');
            AvailabilityFilterModel filter = new()
            {
                Start = DateTime.TryParse(dates[0], out var start) ? start : DateTime.MinValue,
                End = DateTime.TryParse(dates[1], out var end) ? end : DateTime.MinValue
            };

            return filter;
        }

        public AvailabilityFilterModel Parse(string checkIn, string checkOut)
        {
            AvailabilityFilterModel filter = new()
            {
                Start = DateTime.TryParse(checkIn, out var start) ? start : DateTime.MinValue,
                End = DateTime.TryParse(checkOut, out var end) ? end : DateTime.MinValue
            };

            return filter;
        }

        public string GetFilterQuery(AvailabilityFilterModel filter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= filter.Days; i++)
            {
                if (i == 0) stringBuilder.Append($"(pricing_availabilities.date is not null AND pricing_availabilities.date = '{filter.Start.AddDays(i):s}' AND pricing_availabilities.is_available = true AND pricing_availabilities.check_in_allowed = true AND pricing_availabilities.min_stay_night <= {filter.Days})");
                else if (i == filter.Days) stringBuilder.Append($" AND (pricing_availabilities.date is not null AND pricing_availabilities.date = '{filter.Start.AddDays(i):s}' AND (pricing_availabilities.check_out_allowed = true))");
                else stringBuilder.Append($" AND (pricing_availabilities.date is not null AND pricing_availabilities.date = '{filter.Start.AddDays(i):s}' AND pricing_availabilities.is_available = true)");
            }

            return stringBuilder.ToString();
        }
    }
}
