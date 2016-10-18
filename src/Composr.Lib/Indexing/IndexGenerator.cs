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
        private IBlogRepository blogRepository;
        private IPostRepository postRepository;

        public IndexGenerator(IIndexWriter writer, IBlogRepository blogRepository, IPostRepository postRepository)
        {
            this.writer = writer;
            this.blogRepository = blogRepository;
            this.postRepository = postRepository;
        }

        public void BuildIndex(Blog blog)
        {            
            ClearIndexDirectory(blog);
            
            foreach (var locale in Enum.GetValues(typeof(Locale)))
            {
                var blg = new Blog() { Id = blog.Id, Locale = (Locale)locale }; //make a shallow copy to avoid DI MemoryCache corruption
                AddPostsToIndex(blg);
                AddCategoriesToIndex(blg);
            }

            SearchService.ReloadIndex();
        }

        private void AddCategoriesToIndex(Blog blog)
        {
            var categories = blogRepository.GetCategoryPages(blog);
            writer.IndexCategories(categories);
        }

        private void AddPostsToIndex(Blog blog)
        {
            postRepository.Locale = blog.Locale.Value;
            var posts = postRepository.GetPublishedPosts(new Filter { Limit = int.MaxValue });
            var synonymEngine = new ComposrSynonymEngine(blogRepository.GetSynonyms(blog));
            writer.IndexPosts(posts, synonymEngine);
        }

        private static void ClearIndexDirectory(Blog blog)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Settings.IndexDirectory))
                System.IO.File.Delete(file);
        }
    }
}
