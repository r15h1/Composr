﻿using Composr.Core;
using Composr.Lib.Util;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Composr.Lib.Indexing
{
    public class SearchEngine
    {
        private static IndexReader reader;
        private static Directory directory;

        static SearchEngine()
        {
            directory = new RAMDirectory(FSDirectory.Open(Configuration.IndexDirectory));
            reader = IndexReader.Open(directory, true);
        }

        public IList<SearchResult> Search(SearchCriteria criteria)
        {
            IndexSearcher searcher = new IndexSearcher(reader);
            Query query = CreateQuery(criteria);
            TopDocs docs = searcher.Search(query, 25);
            CompileOptions options = string.IsNullOrWhiteSpace(criteria.URN) ? CompileOptions.Exclude_Post_Body : CompileOptions.Include_Post_Body;
            return CompileResults(searcher, docs, options);
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

            if(options == CompileOptions.Include_Post_Body)
                result.Body = doc.Get(IndexFields.PostBody);
            else
                result.Snippet = doc.Get(IndexFields.PostSnippet);

            return result;
        }

        private Query CreateQuery(SearchCriteria criteria)
        {
            BooleanQuery query = new BooleanQuery();
            TermQuery q = null;

            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                q = new TermQuery(new Term(IndexFields.PostTitle, criteria.SearchTerm.ToLowerInvariant()));
                q.Boost = 1.5f;
                query.Add(q, Occur.SHOULD);

                q = new TermQuery(new Term(IndexFields.PostBody, criteria.SearchTerm.ToLowerInvariant()));
                q.Boost = 1.0f;
                query.Add(q, Occur.SHOULD);
            }

            if (!string.IsNullOrWhiteSpace(criteria.URN))
            {
                q = new TermQuery(new Term(IndexFields.PostURN, criteria.URN.ToLowerInvariant()));
                q.Boost = 1.5f;
                query.Add(q, Occur.MUST);
            }

            q = new TermQuery(new Term(IndexFields.BlogID, criteria.BlogID.ToString()));
            query.Add(q, Occur.MUST);

            q = new TermQuery(new Term(IndexFields.Locale, criteria.Locale.ToString()));
            query.Add(q, Occur.MUST);

            return query;
        }
    }

    public enum CompileOptions
    {
        Exclude_Post_Body,
        Include_Post_Body
    }
}