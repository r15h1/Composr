using Composr.Core;
using Composr.Lib.Indexing;
using System.Collections.Generic;

namespace Composr.Lib.Services
{
    public class SearchService : ISearchService
    {       

        public IList<SearchResult> Search(SearchCriteria criteria)
        {
            return new SearchEngine().Search(criteria);
        }
    }
}
