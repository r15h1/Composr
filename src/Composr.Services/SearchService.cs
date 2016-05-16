using Composr.Core;
using Composr.Core.Services;
using Composr.Indexing;
using System;
using System.Collections.Generic;

namespace Composr.Services
{
    public class SearchService : ISearchService
    {
        public IList<SearchResult> Search(SearchCriteria criteria)
        {
            return new SearchEngine().Search(criteria);
        }
    }
}
