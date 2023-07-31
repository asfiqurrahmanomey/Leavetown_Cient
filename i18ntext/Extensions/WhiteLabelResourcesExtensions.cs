namespace Leavetown.Client.i18ntext.Extensions
{
    public static class WhiteLabelResourcesExtensions
    {
        public static string GetDaysValue(this I18nText.SharedResources sharedResources, int days) =>
            GetCountValue(days, sharedResources.DaySingular, sharedResources.DayPlural);

        public static string GetHoursValue(this I18nText.SharedResources sharedResources, int hours) =>
            GetCountValue(hours, sharedResources.HourSingular, sharedResources.HourPlural);

        private static string GetCountValue(int count, string singlularFormat, string pluralFormat)
        {
            string format = Math.Abs(count) == 1 ? singlularFormat : pluralFormat;

            string text = string.Format(format, count);
            return text;
        }
    }
}
