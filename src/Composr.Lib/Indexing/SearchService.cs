using Composr.Core;
using Composr.Lib.Util;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Composr.Lib.Indexing
{
    public class SearchService : ISearchService
    {
        private static Lucene.Net.Store.Directory directory;
        private IndexSearcher searcher;

        static SearchService()
        {
            directory = new RAMDirectory(FSDirectory.Open(Settings.IndexDirectory));
        }

        public SearchService()
        {
            searcher = new IndexSearcher(directory);
        }

        public static void ReloadIndex()
        {
            directory = new RAMDirectory(FSDirectory.Open(Settings.IndexDirectory));
        }

        public SearchResults Search(SearchCriteria criteria)
        {
            Query query = CreateQuery(criteria);
            TopDocs docs = Search(criteria, query);            
            return CompileResults(docs, criteria);
        }

        private TopDocs Search(SearchCriteria criteria, Query query)
        {
            if (criteria.SearchSortOrder == SearchSortOrder.MostRecent)
                return searcher.Search(query, null, Settings.MaxSearchResultsCount, new Sort(new SortField(IndexFields.PostDatePublishedTicks, SortField.STRING, true)));

            return searcher.Search(query, Settings.MaxSearchResultsCount);
        }

        private SearchResults CompileResults(TopDocs docs, SearchCriteria criteria)
        {
            SearchResults results = new SearchResults();
            CompileOptions options = criteria.SearchType == SearchType.URN ? CompileOptions.Include_Post_Body : CompileOptions.Exclude_Post_Body;

            var hits = GetDocList(docs, criteria);
            foreach (var doc in hits)
                results.Hits.Add(CompileResult(doc.Doc, options));            

            results.HitsCount = docs.TotalHits;
            return results;
        }

        private IEnumerable<ScoreDoc> GetDocList(TopDocs docs, SearchCriteria criteria)
        {
            if (criteria.SearchType == SearchType.MoreLikeThis)
                return docs.ScoreDocs.Where(d => d.Doc != criteria.DocumentId).Skip(criteria.Start).Take(criteria.Limit);

            return docs.ScoreDocs.Skip(criteria.Start).Take(criteria.Limit);
        }

        private Hit CompileResult(int documentId, CompileOptions options)
        {
            Hit result = new Hit();
            Document doc = searcher.Doc(documentId);
            result.DocumentId = documentId;
            result.DatePublished = doc.Get(IndexFields.PostDatePublished);
            result.Id = doc.Get(IndexFields.PostID);
            result.Title = doc.Get(IndexFields.Title);
            result.URN = doc.Get(IndexFields.URN);
            result.MetaDescription = doc.Get(IndexFields.PostMetaDescription);
            result.Yield = doc.Get(IndexFields.Yield);
            result.Tags = doc.Get(IndexFields.Tags);

            if (options == CompileOptions.Include_Post_Body)
            {
                result.Body = doc.Get(IndexFields.PostBody);
                result.StructuredDataJsonLD = doc.Get(IndexFields.StructuredDataJsonLD);
            }
            else
                result.Snippet = doc.Get(IndexFields.PostSnippet);

            if (!string.IsNullOrWhiteSpace(doc.Get(IndexFields.ImageUrl)))
            {
                result.PostImage = new PostImage();
                result.PostImage.Url = doc.Get(IndexFields.ImageUrl);
                result.PostImage.Caption = doc.Get(IndexFields.ImageCaption);
            }

            result.Translations = new Dictionary<Locale, string>();
            foreach (var loc in Enum.GetValues(typeof(Locale)))
            {
                var locale = ((Locale)loc);
                string translated = doc.Get(locale.ToString().ToLowerInvariant());
                if (!string.IsNullOrWhiteSpace(translated)) result.Translations.Add(locale, translated);
            }

            return result;
        }

        private Query CreateQuery(SearchCriteria criteria)
        {
            QueryBuilder builder = new QueryBuilder(criteria);
            return builder.Build();
        }

        public SearchResults GetMoreLikeThis(SearchCriteria criteria)
        {
            ISet<string> stopwords = Cache.GetStopWords(new Blog(criteria.BlogID) { Locale = criteria.Locale});
            var builder = new QueryBuilder(criteria) { IndexReader = searcher.IndexReader, StopWords = stopwords };
            var query = builder.Build();            
            TopDocs docs = Search(criteria, query);            
            return  CompileResults(docs, criteria);
        }
    }

    public enum CompileOptions
    {
        Exclude_Post_Body,
        Include_Post_Body
    }
}

