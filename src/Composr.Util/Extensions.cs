
using System;

namespace Composr.Util
{
    public static class Extensions
    {
        /// <summary>
        /// valid characters are a-z, A-Z, 0-9, underscore and whitespace
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAlphaNumericWhiteSpaceUnderscore(this string s)
        {
            return !string.IsNullOrWhiteSpace(s) && !System.Text.RegularExpressions.Regex.IsMatch(s, @"[^\w\s]");
        }

        /// <summary>
        /// check whether the provided string is a valid url
        /// </summary>
        public static bool IsValidHttpUrl(this string url)
        {
            Uri validatedUri;

            if (Uri.TryCreate(url, UriKind.Absolute, out validatedUri)) //.NET URI validation.
            {
                return (validatedUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || validatedUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
    }
}