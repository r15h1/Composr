using Composr.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Composr.Core;

namespace Composr.Services
{
    public class SearchService : ISearchService
    {
        public IList<Post> Search(Blog blog, string searchTerm, SearchSortOrder sort = SearchSortOrder.BestMatch)
        {
            throw new NotImplementedException();
        }
    }
}
