using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace Leavetown.Client.Utilities.Extensions
{
    public static class UriExtensions
    {
        public static string? GetLanguageCodeFromUri(this Uri uri)
        {
            var languageCode = uri.Segments.Length > 1 ? uri.Segments[1].Trim('/') : string.Empty;
            if(ValidateLanguageCode(languageCode)) return languageCode;
            return null;
        }
        
        private static bool ValidateLanguageCode(string languageCode)
        {
            Regex regex = new("^([a-z][a-z])$");
            return languageCode.Length == 2 && regex.IsMatch(languageCode) && CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Any(culture => string.Equals(culture.Name, languageCode, StringComparison.CurrentCultureIgnoreCase));
        }

        public static Uri RemoveQueryStringByKey(this Uri uri, string key)
        {
            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            string result = newQueryString.Count > 0 ? $"{pagePathWithoutQueryString}?{newQueryString}" : pagePathWithoutQueryString;

            return new Uri(result);
        }
    }
}
