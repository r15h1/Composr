using Composr.Core;
using Composr.Core;
using FizzWare.NBuilder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Mock.Repositories
{
    public class BlogRepository : IRepository<Blog>
    {
        static List<Blog> blogs;

        static BlogRepository()
        {
            Initialize();
        }

        private static void Initialize()
        {
            blogs = (List<Blog>)Builder<Blog>
                        .CreateListOfSize(4)                        
                        .Build();
            
        }

        public BlogRepository()
        {
            Locale = Locale.EN;
        }

        public Locale Locale { get; set; }

        public int Count(string criteria)
        {
            return blogs.FindAll(x => x.Name.ToLower().Contains(criteria) || x.Description.Contains(criteria)).Count;
        }

        public void Delete(Blog item)
        {
            throw new NotImplementedException();
        }

        public IList<Blog> Get(Filter filter)
        {
            if (filter == null || string.IsNullOrWhiteSpace(filter.Criteria)) return blogs;
            return blogs.FindAll(x => x.Name.ToLower().Contains(filter.Criteria) || x.Description.Contains(filter.Criteria));
        }

        public Blog Get(int id)
        {
            return blogs.SingleOrDefault(x => x.Id == id);
        }

        public int Save(Blog item)
        {
            throw new NotImplementedException();
        }
    }
}
