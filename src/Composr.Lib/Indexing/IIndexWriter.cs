using Composr.Core;
using System.Collections.Generic;

namespace Composr.Lib.Indexing
{
    public interface IIndexWriter
    {
        /// <summary>
        /// generates an index with all these posts
        /// </summary>
        /// <param name="posts"></param>
        /// <param name="SynonymEngine">the synonym engine to use</param>
        void GenerateIndex(IList<Post> posts, ISynonymEngine SynonymEngine);        
    }
}
