using Composr.Core;
using Composr.Core.Repositories;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Mock.Repositories
{
    public class PostRepository : IRepository<Post>
    {
        
        private Blog blog;
        static List<Post> posts;

        static PostRepository()
        {
            Initialize();
        }

        private static void Initialize()
        {
            posts = (List<Post>)Builder<Post>
                        .CreateListOfSize(30)
                        .WhereTheFirst<Post>(10).AreConstructedUsing(() => new Post(new Blog(1)))
                        .AndTheNext<Post>(8).AreConstructedUsing(() => new Post(new Blog(2)))
                        .AndTheNext<Post>(7).AreConstructedUsing(() => new Post(new Blog(3)))
                        .AndTheNext<Post>(5).AreConstructedUsing(() => new Post(new Blog(4)))
                        .Build();
        }

        public PostRepository(Blog blog)
        {
            this.blog = blog;
        }

        public Locale Locale
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count(string criteria)
        {
            throw new NotImplementedException();
        }

        public void Delete(Post item)
        {
            throw new NotImplementedException();
        }

        public IList<Post> Get(Filter filter)
        {
            if (filter == null || string.IsNullOrWhiteSpace(filter.Criteria)) 
                return posts.FindAll(p => p.Blog.Id == blog.Id);

            return posts.FindAll(p => p.Blog.Id == blog.Id && p.Title.Contains(filter.Criteria));
        }

        public Post Get(int id)
        {
            return posts.SingleOrDefault(p => p.Blog.Id == blog.Id && p.Id == id);
        }

        public int Save(Post item)
        {
            throw new NotImplementedException();
        }
    }
}
