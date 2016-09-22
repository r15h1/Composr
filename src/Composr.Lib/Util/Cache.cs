using Composr.Core;
using Composr.Repository.Sql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Composr.Lib.Util
{
    public class Cache
    {
        private static ConcurrentDictionary<Blog, ISet<string>> stopwords = new ConcurrentDictionary<Blog, ISet<string>>(new BlogComparer());
        internal static ISet<string> GetStopWords(Blog blog)
        {
            if (!stopwords.ContainsKey(blog)) TryFetchStopWordsFromDB(blog);

            return stopwords[blog];
        }

        private static void TryFetchStopWordsFromDB(Blog blog)
        {
            BlogRepository repository = new BlogRepository(new Specifications.MinimalBlogSpecification());
            ISet<string> sw = repository.GetStopWords(blog);
            if (sw != null) stopwords.TryAdd(blog, sw);
        }
    }

    public class BlogComparer : EqualityComparer<Blog>
    {
        public override bool Equals(Blog x, Blog y)
        {
            return x.Locale == y.Locale && x.Id == y.Id;                        
        }

        public override int GetHashCode(Blog obj)
        {
            return obj.Locale.GetHashCode() + (string.IsNullOrWhiteSpace(obj.Url) ? 0 : obj.Url.GetHashCode());
        }
    }
}
