namespace Composr.Core
{
    public interface IUrlMapper
    {
        bool HasRedirectUrl(string url);

        string GetRedirectUrl(string originalUrl);

        bool HasTranslatedUrl(Locale sourceLocale, Locale targetLocale, string url);

        string GetTranslatedUrl(Locale sourceLocale, Locale targetLocale, string url);
    }
}
