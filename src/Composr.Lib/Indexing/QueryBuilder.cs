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
            finalQuery.Add(new TermQuery(new Term(IndexFields.BlogID, BlogID.ToString())), Occur.MUST);
            finalQuery.Add(new TermQuery(new Term(IndexFields.Locale, Locale.ToString().ToLowerInvariant())), Occur.MUST);
            if (!string.IsNullOrWhiteSpace(SearchTerm)) CreateSearchTermQuery();
            return finalQuery;
        }

        public int BlogID { get; set; }
        public Locale Locale { get; set; }
        public string SearchTerm { get; set; }
        public SearchType SearchType { get; set; }

        private void CreateSearchTermQuery()
        {
            if (SearchType == SearchType.AutoComplete || SearchType == SearchType.Default) CreateDefaultSearchQuery();
            else if (SearchType == SearchType.URN) CreateURNQuery();
        }        

        private void CreateDefaultSearchQuery()
        {
            BooleanQuery q1 = new BooleanQuery();
            foreach (var term in SplitSearchTerm())
            {
                q1.Add(CreateWildCardQuery(IndexFields.PostTitle, SearchTerm, 3.0f), Occur.SHOULD);
                if(SearchType == SearchType.Default) q1.Add(CreateWildCardQuery(IndexFields.PostMetaDescription, SearchTerm, 1.0f), Occur.SHOULD);
            }
            q1.Add(BoostPostWithImages(), Occur.SHOULD);
            finalQuery.Add(q1, Occur.MUST);
        }

        private void CreateURNQuery()
        {
            TermQuery tq = new TermQuery(new Term(IndexFields.PostURN, SearchTerm.ToLowerInvariant().TrimEnd('/')));
            tq.Boost = 1.5f;
            finalQuery.Add(tq, Occur.MUST);
        }

        private Query CreateWildCardQuery(string field, string term, float boost)
        {
            WildcardQuery w = new WildcardQuery(new Term(field, $"{term}*"));
            w.Boost = 3.0f;
            return w;
        }

        private Query BoostPostWithImages()
        {
            TermQuery tq = new TermQuery(new Term(IndexFields.HasImage, "y"));
            tq.Boost = 1.5f;
            return tq;
        }

        private IEnumerable<string> SplitSearchTerm()
        {
            return SearchTerm.ToLowerInvariant().Trim().Split(new char[] { ' ', ',', '-', '.', ';', ':', '+', '*', '%', '&' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
