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
        void IndexPosts(IList<Post> posts, ISynonymEngine SynonymEngine);

        /// <summary>
        /// adds categories to an index
        /// </summary>
        /// <param name=""></param>
        void IndexCategories(IList<Category> categories);        
    }
}
