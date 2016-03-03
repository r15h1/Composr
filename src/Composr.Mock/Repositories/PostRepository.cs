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

        static PostRepository()
        {
            
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
            throw new NotImplementedException();
        }

        public Post Get(int id)
        {
            throw new NotImplementedException();
        }

        public int Save(Post item)
        {
            throw new NotImplementedException();
        }
    }
}
