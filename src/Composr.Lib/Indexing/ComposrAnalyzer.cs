using Composr.Core;
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
            var tokenstream = base.TokenStream(fieldName, new HTMLStripCharFilter(chStream));
            if (SynonymEngine != null)
                tokenstream = new SynonymFilter(tokenstream, SynonymEngine);

            return new PorterStemFilter(tokenstream);
        }

        public ISynonymEngine SynonymEngine { get; set; }
    }
}