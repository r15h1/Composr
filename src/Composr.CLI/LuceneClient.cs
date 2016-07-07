using System.Collections.Generic;
using Composr.Core;
using Microsoft.Extensions.Logging;
using Composr.Lib.Util;
using Composr.Lib.Indexing;

namespace Composr.CLI
{
    public class LuceneClient
    {
        private ILogger logger = Logging.CreateLogger<LuceneClient>();

        public LuceneClient()
        {
        }

        internal void GenerateIndex()
        {
            logger.LogInformation("getting list of blogs");
            IBlogRepository repo = new Composr.Repository.Sql.BlogRepository();
            IList<Blog> blogs = repo.Get(null);
            foreach (var blog in blogs) GenerateIndex(blog);
        }

        private void GenerateIndex(Blog blog)
        {
            ClearIndexDirectory(blog);
            logger.LogInformation($"generating index for {blog.Name}");
            IPostRepository repo = new Composr.Repository.Sql.PostRepository(blog);
            IList<Post> posts = repo.GetPublishedPosts(new Filter { Limit = int.MaxValue});
            IIndexWriter writer = new LuceneIndexWriter(blog);

            if(!System.IO.Directory.Exists(Settings.IndexDirectory.TrimEnd('\\') + $"\\{blog.Id}"))
                System.IO.Directory.CreateDirectory(Settings.IndexDirectory.TrimEnd('\\') + $"\\{blog.Id}");

            writer.GenerateIndex(posts);
        }

        private static void ClearIndexDirectory(Blog blog)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Settings.IndexDirectory.TrimEnd('\\') + $"\\{blog.Id}"))
                System.IO.File.Delete(file);
        }
    }
}
