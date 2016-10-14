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
                //make a shallow copy to avoid DI MemoryCache corruption
                var blg = new Blog() { Id = blog.Id, Locale = (Locale)locale };
                postRepository.Locale = (Locale)locale;
                var posts = postRepository.GetPublishedPosts(new Filter { Limit = int.MaxValue });
                var synonymEngine = new ComposrSynonymEngine(blogRepository.GetSynonyms(blg));
                writer.GenerateIndex(posts, synonymEngine);
            }
            
            SearchService.ReloadIndex();
        }

        private static void ClearIndexDirectory(Blog blog)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Settings.IndexDirectory))
                System.IO.File.Delete(file);
        }
    }
}
