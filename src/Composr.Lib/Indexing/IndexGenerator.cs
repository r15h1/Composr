using Composr.Core;
using Composr.Lib.Specifications;
using Composr.Lib.Util;
using System.Collections.Generic;

namespace Composr.Lib.Indexing
{
    public class IndexGenerator: IIndexGenerator
    {
        private IIndexWriter writer;

        public IndexGenerator(IIndexWriter writer)
        {
            this.writer = writer;
        }

        public void BuildIndex(Blog blog)
        {
            ClearIndexDirectory(blog);
            IPostRepository repo = new Composr.Repository.Sql.PostRepository(blog, new PostSpecification());
            IList<Post> posts = repo.GetPublishedPosts(new Filter { Limit = int.MaxValue });
            writer.GenerateIndex(posts);
            SearchEngine.ReloadIndex();
        }

        private static void ClearIndexDirectory(Blog blog)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Settings.IndexDirectory))
                System.IO.File.Delete(file);
        }
    }
}
