using Composr.Core;
using System.Collections.Concurrent;

namespace Composr.Lib.Util
{
    public class RedirectionMapper:IRedirectionMapper
    {
        private ConcurrentDictionary<string, string> redirections;

        public RedirectionMapper()
        {
            Initialize();
        }

        public bool CanResolve(string url)
        {
            return redirections.ContainsKey(url);
        }

        public string MapToRedirectUrl(string url)
        {
            if (redirections.ContainsKey(url))
                return redirections[url];

            return null;
        }

        private void Initialize()
        {
            redirections = new ConcurrentDictionary<string, string>();
            redirections.TryAdd("2016/04/rice-vermicelli-soup-with-chicken-and.html", "/mauritius/cooking/rice-vermicelli-soup-recipe");
        }
    }
}
