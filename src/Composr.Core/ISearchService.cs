using System.Collections.Generic;

namespace Composr.Core
{
    /// <summary>
    /// gets data for front end - with advanced search capabilities
    /// notto be confused with RepoService, which is intended for backend 
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// get a list of posts that are indexed and published
        /// </summary>
        /// <returns></returns>
        SearchResults Search(SearchCriteria criteria);

        /// <summary>
        /// gets a list of posts that are similar to the one passed as parameter
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        SearchResults GetMoreLikeThis(SearchCriteria criteria);
    }    
}