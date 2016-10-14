using System.Collections.Generic;

namespace Composr.Core
{
    public interface IBlogRepository:IRepository<Blog>
    {
        ISet<string> GetStopWords(Blog blog);

        IDictionary<string, IList<string>> GetSynonyms(Blog blog);
    }
}
