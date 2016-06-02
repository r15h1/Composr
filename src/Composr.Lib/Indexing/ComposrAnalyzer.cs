using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System.IO;

namespace Composr.Lib.Indexing
{
    internal class ComposrAnalyzer:StandardAnalyzer
    {
        public ComposrAnalyzer(Lucene.Net.Util.Version version) : base(version){}

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            CharStream chStream = CharReader.Get(reader);
            HTMLStripCharFilter filter = new HTMLStripCharFilter(chStream);
            return base.TokenStream(fieldName, filter);
        }
    }
}