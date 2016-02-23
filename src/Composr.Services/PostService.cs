using Composr.Core;
using Composr.Core.Repositories;
using Composr.Core.Specifications;
using Composr.Specifications;
using System;
using System.Collections.Generic;

namespace Composr.Services
{
    public class PostService
    {
        private PostRepository repository;
        private ISpecification<Post> spec = new MinimalPostSpecification();

        public PostService(PostRepository repository)
        {
            if (repository == null) throw new ArgumentNullException();
            this.repository = repository;
            this.repository.Locale = Composr.Util.RuntimeInfo.ExecutingLocale;
        }

        public int Save(Post post)
        {
            if (post == null) throw new ArgumentNullException();
            var compliance = spec.EvaluateCompliance(post);
            if (!compliance.IsSatisfied) throw new SpecificationException(string.Join(Environment.NewLine, compliance.Errors));
            return repository.Save(post);            
        }

        public Post Get(int id)
        {
            return repository.Get(id);
        }

        public IList<Post> Get(Filter filter)
        {
            return repository.Get(filter);
        }

        public int Count(string criteria)
        {
            return repository.Count(criteria);
        }

        public void Delete(Post post)
        {
            if (post == null || !post.PostID.HasValue) throw new ArgumentNullException();
            repository.Delete(post);
        }
    }
}