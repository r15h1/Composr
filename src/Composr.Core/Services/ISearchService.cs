﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core.Services
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
        IList<Post> Search(Blog blog, string searchTerm, SearchSortOrder sort = SearchSortOrder.BestMatch);
    }
    
    public enum SearchSortOrder
    {
        Title,
        DatePublished,
        BestMatch
    }
}