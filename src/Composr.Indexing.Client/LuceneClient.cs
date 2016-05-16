﻿using System.Collections.Generic;
using Composr.Core.Repositories;
using Composr.Core;
using System.Linq;
using Microsoft.Extensions.Logging;
using Composr.Util;

namespace Composr.Indexing.Client
{
    public class LuceneClient
    {
        private ILogger logger = Logging.CreateLogger<LuceneClient>();
        private IIndexWriter writer;

        public LuceneClient(IIndexWriter writer)
        {
            this.writer = writer; 
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
            logger.LogInformation($"generating index for {blog.Name}");
            IPostRepository repo = new Composr.Repository.Sql.PostRepository(blog);
            IList<Post> posts = repo.GetPublishedPosts(null);
            writer.GenerateIndex(posts);
        }
    }
}