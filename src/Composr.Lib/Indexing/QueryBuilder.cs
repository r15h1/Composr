using Composr.Core;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using System;
using System.Collections.Generic;
using System.IO;

namespace Composr.Lib.Indexing
{
    public class QueryBuilder
    {
        private SearchCriteria criteria;
        private ComposrAnalyzer analyzer;

        public QueryBuilder(SearchCriteria criteria)
        {
            if (criteria == null) throw new ArgumentNullException();
            this.criteria = criteria;
            analyzer = new Indexing.ComposrAnalyzer(Lucene.Net.Util.Version.LUCENE_30);            
        }

        public Query Build()
        {
            if (criteria.SearchType == SearchType.MoreLikeThis)
                return CreateMoreLikeThisQuery();

            return BuildQuery();
        }

        private Query BuildQuery()
        {
            BooleanQuery finalQuery = new BooleanQuery();
            finalQuery.Add(CreateTermQuery(IndexFields.BlogID, criteria.BlogID.ToString(), 1.0f), Occur.MUST);
            if (criteria.Locale.HasValue) finalQuery.Add(CreateTermQuery(IndexFields.Locale, criteria.Locale.Value.ToString().ToLowerInvariant(), 1.0f), Occur.MUST);
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm)) finalQuery.Add(CreateSearchTermQuery(), Occur.MUST);
            if (!string.IsNullOrWhiteSpace(criteria.Tags)) finalQuery.Add(CreateTagQuery(), Occur.MUST);
            finalQuery.Add(BoostPostWithImages(), Occur.SHOULD);
            return finalQuery;
        }

        public IndexReader IndexReader { get; set; }
        public ISet<string> StopWords { get; set; }

        private Query CreateSearchTermQuery()
        {
            if (criteria.SearchType == SearchType.URN)
                return CreateURNQuery();

            return CreateDefaultSearchQuery();
        }

        private Query CreateDefaultSearchQuery()
        {
            BooleanQuery query = new BooleanQuery();
            foreach(var term in Split(criteria.SearchTerm))
            {
                query.Add(CreateWildCardQuery(IndexFields.Title, term, 3.0f), Occur.SHOULD);
                if(criteria.SearchType == SearchType.Default) query.Add(CreateWildCardQuery(IndexFields.PostMetaDescription, term, 1.0f), Occur.SHOULD);
            }
            return query;
        }

        private Query CreateURNQuery()
        {
            return CreateTermQuery(IndexFields.URN, criteria.SearchTerm.ToLowerInvariant().TrimEnd('/'), 1.5f);
        }        
        
        private Query BoostPostWithImages()
        {
            return CreateTermQuery(IndexFields.HasImage, "y", 1.5f);
        }

        private Query CreateTagQuery()
        {
            BooleanQuery q1 = new BooleanQuery();
            foreach (var term in Split(criteria.Tags))
                q1.Add(CreateTermQuery(IndexFields.Tags, term, 1.5f), Occur.MUST);

            return q1;
        }

        private IEnumerable<string> Split(string term)
        {
            List<string> terms = new List<string>();
            foreach(var t in term.ToLowerInvariant().Trim().Split(new char[] { ' ', ',', '-', '.', ';', ':', '+', '*', '%', '&' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tokenStream = analyzer.TokenStream(null, new StringReader(t));
                var termAttr = tokenStream.GetAttribute<ITermAttribute>();
                while (tokenStream.IncrementToken())
                    terms.Add(termAttr.Term);
            }

            return terms;
        }

        private Query CreateWildCardQuery(string field, string term, float boost)
        {
            return new WildcardQuery(new Term(field, $"{term}*")) { Boost = boost };
        }

        private Query CreateTermQuery(string field, string term, float boost)
        {
            return new TermQuery(new Term(field, term)) { Boost = boost };
        }

        private Query CreateMoreLikeThisQuery()
        {
            MoreLikeThis mlt = new MoreLikeThis(IndexReader);
            mlt.Analyzer = analyzer;
            mlt.SetFieldNames(new string[] { IndexFields.PostBody, IndexFields.PostMetaDescription, IndexFields.Title });
            mlt.MinTermFreq = 1;
            mlt.MinDocFreq = 1;
            mlt.MinWordLen = 3;
            mlt.Boost = true;
            mlt.SetStopWords(StopWords);

            Query query = mlt.Like(criteria.DocumentId);
            BooleanQuery finalQuery = new BooleanQuery();
            finalQuery.Add(query, Occur.MUST);
            finalQuery.Add(new TermQuery(new Term(IndexFields.BlogID, criteria.BlogID.ToString().ToLowerInvariant())), Occur.MUST);
            finalQuery.Add(new TermQuery(new Term(IndexFields.Locale, criteria.Locale.ToString().ToLowerInvariant())), Occur.MUST);
            finalQuery.Add(BoostPostWithImages(), Occur.SHOULD);
            return finalQuery;
        }
    }
}
