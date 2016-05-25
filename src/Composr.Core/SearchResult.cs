using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core
{
    /// <summary>
    /// data structure to represent a post in the form of a search result
    /// </summary>
    public class SearchResult
    {
        public string Title { get; set; }
        public string Snippet { get; set; }
        public string URN { get; set; }
        public string Id { get; set; }
        public string DatePublished { get; set; }
        public string Body{ get; set; }
        public string Thumbnail { get; set; }
    }

    public enum SearchSortOrder
    {
        Title,
        DatePublished,
        BestMatch
    }

    public class SearchCriteria
    {
        public SearchCriteria()
        {
            SearchSortOrder = SearchSortOrder.BestMatch;
        }

        public int BlogID { get; set; }
        public Locale Locale { get; set; }
        public string SearchTerm { get; set; }
        public SearchSortOrder SearchSortOrder { get; set; }
        public string URN { get; set; }

    }
}
