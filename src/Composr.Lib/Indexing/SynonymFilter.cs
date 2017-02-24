using Composr.Core;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Lib.Indexing
{
    internal class SynonymFilter : TokenFilter
    {
        private ITermAttribute termAtt;
        private ISynonymEngine synonymEngine;
        private PositionIncrementAttribute posAtt;
        private Stack<string> currentSynonyms;
        private State currentState;
        public SynonymFilter(TokenStream input, ISynonymEngine synonymEngine) : base(input)
        {
            termAtt = AddAttribute<ITermAttribute>();
            posAtt = (PositionIncrementAttribute)AddAttribute<IPositionIncrementAttribute>();
            currentSynonyms = new Stack<string>();
            this.synonymEngine = synonymEngine;
        }
        public override bool IncrementToken()
        {
            if (currentSynonyms.Count > 0)
            {
                string synonym = currentSynonyms.Pop();
                RestoreState(currentState);
                termAtt.SetTermBuffer(synonym);
                posAtt.PositionIncrement = 0;
                return true;
            }
            if (!input.IncrementToken()) return false;
            string currentTerm = termAtt.Term;
            if (currentTerm != null)
            {
                var synonyms = synonymEngine.GetSynonyms(currentTerm);
                if (synonyms == null || !synonyms.Any()) return true;

                foreach (var synonym in synonyms)
                    currentSynonyms.Push(synonym.ToLower());                
            }
            currentState = CaptureState();
            return true;
        }
    }
}
