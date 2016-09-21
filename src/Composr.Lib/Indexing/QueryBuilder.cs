using Composr.Core;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Similar;
using System;
using System.Collections.Generic;

namespace Composr.Lib.Indexing
{
    public class QueryBuilder
    {
        private SearchCriteria criteria;   

        public QueryBuilder(SearchCriteria criteria)
        {
            if (criteria == null) throw new ArgumentNullException();
            this.criteria = criteria;
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
                query.Add(CreateWildCardQuery(IndexFields.PostTitle, term, 3.0f), Occur.SHOULD);
                if(criteria.SearchType == SearchType.Default) query.Add(CreateWildCardQuery(IndexFields.PostMetaDescription, term, 1.0f), Occur.SHOULD);
            }
            return query;
        }

        private Query CreateURNQuery()
        {
            return CreateTermQuery(IndexFields.PostURN, criteria.SearchTerm.ToLowerInvariant().TrimEnd('/'), 1.5f);
        }        
        
        private Query BoostPostWithImages()
        {
            return CreateTermQuery(IndexFields.HasImage, "y", 1.5f);
        }

        private Query CreateTagQuery()
        {
            BooleanQuery q1 = new BooleanQuery();
            foreach (var term in Split(criteria.Tags))
                q1.Add(CreateTermQuery(IndexFields.Tags, term, 1.5f), Occur.SHOULD);

            return q1;
        }

        private IEnumerable<string> Split(string term)
        {
            return term.ToLowerInvariant().Trim().Split(new char[] { ' ', ',', '-', '.', ';', ':', '+', '*', '%', '&' }, StringSplitOptions.RemoveEmptyEntries);
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
            mlt.Analyzer = new ComposrAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            mlt.SetFieldNames(new string[] { IndexFields.PostBody, IndexFields.PostMetaDescription, IndexFields.PostTitle });
            mlt.MinTermFreq = 1;
            mlt.MinDocFreq = 1;
            mlt.MinWordLen = 3;
            mlt.Boost = true;
            mlt.SetStopWords(new HashSet<string> { "ingredients", "salt", "teaspoon", "spoon", "tablespoon", "grams", "wash", "small", "store", "white", "combine", "cup", "leaves", "seed", "finely", "chopped", "powder", "oil", "Instructions", "allow", "serve", "sliced", "minutes" });

            Query query = mlt.Like(criteria.DocumentId);
            BooleanQuery finalQuery = new BooleanQuery();
            finalQuery.Add(query, Occur.MUST);
            finalQuery.Add(new TermQuery(new Term(IndexFields.BlogID, criteria.BlogID.ToString().ToLowerInvariant())), Occur.MUST);
            finalQuery.Add(new TermQuery(new Term(IndexFields.Locale, criteria.Locale.ToString().ToLowerInvariant())), Occur.MUST);
            return finalQuery;
        }
    }
}
