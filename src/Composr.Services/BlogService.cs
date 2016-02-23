using Composr.Core;
using Composr.Core.Repositories;
using Composr.Core.Specifications;
using Composr.Specifications;
using System;
using System.Collections.Generic;

namespace Composr.Services
{
    /// <summary>
    /// facade to manipulate blogs
    /// </summary>
    public class BlogService
    {
        private BlogRepository repository;
        private ISpecification<Blog> spec = new MinimalBlogSpecification();

        public BlogService(BlogRepository repository)
        {
            if (repository == null) throw new ArgumentNullException();
            this.repository = repository;
            this.repository.Locale = Composr.Util.RuntimeInfo.ExecutingLocale;
        }

        public int Save(Blog blog)
        {
            if (blog == null) throw new ArgumentNullException();
            var compliance = spec.EvaluateCompliance(blog);
            if (!compliance.IsSatisfied) throw new SpecificationException(string.Join(Environment.NewLine, compliance.Errors));
            return repository.Save(blog);
        }

        public Blog Get(int id)
        {
            return repository.Get(id);
        }

        public IList<Blog> Get(Filter filter)
        {
            return repository.Get(filter);
        }

        public int Count(string criteria)
        {
            return repository.Count(criteria);
        }

        public void Delete(Blog blog)
        {
            if (blog == null || !blog.BlogID.HasValue) throw new ArgumentNullException();
            repository.Delete(blog);
        }

    }
}