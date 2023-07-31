using Leavetown.Client.Models;
using Leavetown.Client.Services.Contracts;
using Leavetown.Shared.Constants;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace Leavetown.Client.Services
{
    public class GuestFilterService : IGuestFilterService
    {
        public GuestFilterModel Parse(KeyValuePair<string, StringValues> query)
        {
            GuestFilterModel filter = new();
            if (query.Key.Equals(nameof(FilterType.Guests), StringComparison.InvariantCultureIgnoreCase)) filter.GuestCount = Convert.ToInt32(query.Value.ToString());
            if (query.Key.Equals(nameof(FilterType.Adults), StringComparison.InvariantCultureIgnoreCase)) filter.AdultCount = Convert.ToInt32(query.Value.ToString());
            if (query.Key.Equals(nameof(FilterType.Children), StringComparison.InvariantCultureIgnoreCase)) filter.ChildCount = Convert.ToInt32(query.Value.ToString());
            return filter;
        }

        public async Task<GuestFilterModel> ParseAsync(Dictionary<string, StringValues> queryValues) => await Task.Run(() =>
        {
            GuestFilterModel filter = new();

            if (!queryValues.Any()) return filter;

            filter.GuestCount = queryValues.TryGetValue(nameof(FilterType.Guests).ToLower(), out var guestCount) ?
                Convert.ToInt32(guestCount.ToString()) : filter.MinAdults;
            filter.AdultCount = queryValues.TryGetValue(nameof(FilterType.Adults).ToLower(), out var adultCount) ?
                Convert.ToInt32(adultCount.ToString()) : filter.MinAdults;
            filter.ChildCount = queryValues.TryGetValue(nameof(FilterType.Children).ToLower(), out var childCount) ?
                Convert.ToInt32(childCount.ToString()) : 0;

            return filter;
        });

        public string GetFilterQuery(GuestFilterModel filter)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"(number_of_guests >= {filter.GuestCount})");

            return stringBuilder.ToString();
        }
    }
}
