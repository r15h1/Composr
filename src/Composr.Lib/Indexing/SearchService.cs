using Composr.Core;
using System.Collections.Generic;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Composr.Lib.Util;
using System;

namespace Composr.Lib.Indexing
{
    public class SearchService : ISearchService
    {
        private static IndexReader reader;
        private static Directory directory;

        static SearchService()
        {
            directory = new RAMDirectory(FSDirectory.Open(Settings.IndexDirectory));
            reader = IndexReader.Open(directory, true);
        }

        public static void ReloadIndex()
        {
            directory = new RAMDirectory(FSDirectory.Open(Settings.IndexDirectory));
            reader = IndexReader.Open(directory, true);
        }

        public IList<SearchResult> Search(SearchCriteria criteria)
        {
            IndexSearcher searcher = new IndexSearcher(reader);
            Query query = CreateQuery(criteria);
            TopDocs docs = Search(criteria, searcher, query);
            CompileOptions options = criteria.SearchType == SearchType.URN ? CompileOptions.Include_Post_Body : CompileOptions.Exclude_Post_Body;
            return CompileResults(searcher, docs, options);
        }

        private static TopDocs Search(SearchCriteria criteria, IndexSearcher searcher, Query query)
        {
            if (criteria.SearchSortOrder == SearchSortOrder.MostRecent)
                return searcher.Search(query, null, criteria.Limit, new Sort(new SortField(IndexFields.PostDatePublishedTicks, SortField.STRING, true)));

            return searcher.Search(query, criteria.Limit);
        }

        private IList<SearchResult> CompileResults(IndexSearcher searcher, TopDocs docs, CompileOptions options = CompileOptions.Exclude_Post_Body)
        {
            List<SearchResult> results = new List<SearchResult>();
            foreach (var doc in docs.ScoreDocs)
                results.Add(CompileResult(searcher.Doc(doc.Doc), options));

            return results;
        }

        private SearchResult CompileResult(Document doc, CompileOptions options)
        {
            SearchResult result = new SearchResult();
            result.DatePublished = doc.Get(IndexFields.PostDatePublished);
            result.Id = doc.Get(IndexFields.PostID);
            result.Title = doc.Get(IndexFields.PostTitle);
            result.URN = doc.Get(IndexFields.PostURN);
            result.MetaDescription = doc.Get(IndexFields.PostMetaDescription);
            result.Yield = doc.Get(IndexFields.Yield);
            result.Tags = doc.Get(IndexFields.Tags);

            if (options == CompileOptions.Include_Post_Body)
                result.Body = doc.Get(IndexFields.PostBody);
            else
                result.Snippet = doc.Get(IndexFields.PostSnippet);

            if (!string.IsNullOrWhiteSpace(doc.Get(IndexFields.ImageUrl)))
            {
                result.PostImage = new PostImage();
                result.PostImage.Url = doc.Get(IndexFields.ImageUrl);
                result.PostImage.Caption = doc.Get(IndexFields.ImageCaption);
            }


            return result;
        }

        private Query CreateQuery(SearchCriteria criteria)
        {
            QueryBuilder builder = new QueryBuilder()
            {
                BlogID = criteria.BlogID,
                Locale = criteria.Locale,
                SearchType = criteria.SearchType,
                SearchTerm = criteria.SearchTerm,
                Tags = criteria.Tags
            };
            return builder.Build();
        }               
    }

    public enum CompileOptions
    {
        Exclude_Post_Body,
        Include_Post_Body
    }
}

