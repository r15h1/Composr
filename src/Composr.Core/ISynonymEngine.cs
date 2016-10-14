using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core
{
    public interface ISynonymEngine
    {
        IEnumerable<string> GetSynonyms(string word);
    }
}
