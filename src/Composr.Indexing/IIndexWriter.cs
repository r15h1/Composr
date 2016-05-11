using Composr.Core;
using System.Collections.Generic;

namespace Composr.Indexing
{
    public interface IIndexWriter
    {
        ///// <summary>
        ///// adds or updates a post in the index
        ///// </summary>
        ///// <param name="post"></param>
        //void AddOrUpdate(Post post);

        ///// <summary>
        ///// deletes a post from the index
        ///// </summary>
        ///// <param name="post"></param>
        ///// <returns></returns>
        //bool Delete(Post post);

        /// <summary>
        /// generates an index with all these posts
        /// </summary>
        /// <param name="posts"></param>
        void GenerateIndex(IList<Post> posts);
    }
}
