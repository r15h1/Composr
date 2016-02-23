using System.Collections.Generic;

namespace Composr.Core.Repositories
{
    public interface Repository<T>
    {
        Locale Locale { get; set; }
        T Get(int id);
        IList<T> Get(Filter filter);
        int Count(string criteria);
        int Save(T item);
        void Delete(T item);
    }

    public class Filter
    {
        public string Criteria { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }
}