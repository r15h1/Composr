using Composr.Core;
using Composr.Lib.Util;
using System;
using System.Collections.Generic;

namespace Composr.Lib.Indexing
{
    public class ComposrSynonymEngine : ISynonymEngine
    {
        private IDictionary<string, IList<string>> synonyms;
        public ComposrSynonymEngine(IDictionary<string, IList<string>> synonyms)
        {
            if (synonyms == null) throw new ArgumentNullException();
            this.synonyms = synonyms;
        }

        public IEnumerable<string> GetSynonyms(string word)
        {
            if (!word.IsBlank() && synonyms.ContainsKey(word)) return synonyms[word];

            return null;
        }
    }
}
