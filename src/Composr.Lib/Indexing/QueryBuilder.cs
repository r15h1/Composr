using Composr.Core;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Lib.Indexing
{
    public class QueryBuilder
    {
        private BooleanQuery finalQuery;        

        public Query Build()
        {
            finalQuery = new BooleanQuery();
            finalQuery.Add(CreateTermQuery(IndexFields.BlogID, BlogID.ToString(), 1.0f), Occur.MUST);
            if(Locale.HasValue) finalQuery.Add(CreateTermQuery(IndexFields.Locale, Locale.Value.ToString().ToLowerInvariant(), 1.0f), Occur.MUST);
            if (!string.IsNullOrWhiteSpace(SearchTerm)) finalQuery.Add(CreateSearchTermQuery(), Occur.MUST);
            if (!string.IsNullOrWhiteSpace(Tags)) finalQuery.Add(CreateTagQuery(), Occur.MUST);
            finalQuery.Add(BoostPostWithImages(), Occur.SHOULD);
            return finalQuery;
        }

        public int BlogID { get; set; }
        public Locale? Locale { get; set; }
        public string SearchTerm { get; set; }
        public string Tags { get; set; }
        public SearchType SearchType { get; set; }

        private Query CreateSearchTermQuery()
        {
            if (SearchType == SearchType.URN)
                return CreateURNQuery();

            return CreateDefaultSearchQuery();
        }

        private Query CreateDefaultSearchQuery()
        {
            BooleanQuery query = new BooleanQuery();
            foreach(var term in Split(SearchTerm))
            {
                query.Add(CreateWildCardQuery(IndexFields.PostTitle, term, 3.0f), Occur.SHOULD);
                if(SearchType == SearchType.Default) query.Add(CreateWildCardQuery(IndexFields.PostMetaDescription, term, 1.0f), Occur.SHOULD);
            }
            return query;
        }

        private Query CreateURNQuery()
        {
            return CreateTermQuery(IndexFields.PostURN, SearchTerm.ToLowerInvariant().TrimEnd('/'), 1.5f);
        }        
        
        private Query BoostPostWithImages()
        {
            return CreateTermQuery(IndexFields.HasImage, "y", 1.5f);
        }

        private Query CreateTagQuery()
        {
            BooleanQuery q1 = new BooleanQuery();
            foreach (var term in Split(Tags))
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
    }
}
