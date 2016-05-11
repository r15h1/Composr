
using System;
using System.Text.RegularExpressions;

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

        static Regex htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
        public static string StripHTMLTags(this string source)
        {
            return htmlRegex.Replace(source, string.Empty);
        }

        public static string StripLineFeedCarriageReturn(this string source)
        {
            return source.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace("  ", " ");
        }

        static readonly Regex trimmer = new Regex(@"\s\s+");
        public static string StripConsecutiveSpaces(this string source)
        {
            return trimmer.Replace(source, " ");
        }

        public static string GetFirstHtmlParagraph(this string source)
        {
            int index = source.IndexOf("</p>");
            if (index > 0)
                return source.Substring(0, index);

            return source;
        }

    }
}