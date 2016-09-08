using Composr.Core;
using Composr.Lib.Specifications;
using Composr.Lib.Util;
using System;
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
            IBlogRepository blogRepository = new Repository.Sql.BlogRepository(new MinimalBlogSpecification());
            ClearIndexDirectory(blog);
            List<Post> posts = new List<Post>();

            foreach (var locale in Enum.GetValues(typeof(Locale)))
            {
                IPostRepository repo = new Composr.Repository.Sql.PostRepository(blog, new PostSpecification(), blogRepository);
                repo.Locale = (Locale)locale;
                posts.AddRange(repo.GetPublishedPosts(new Filter { Limit = int.MaxValue }));                
            }

            writer.GenerateIndex(posts);
            SearchService.ReloadIndex();
        }

        private static void ClearIndexDirectory(Blog blog)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Settings.IndexDirectory))
                System.IO.File.Delete(file);
        }
    }
}
