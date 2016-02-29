using Composr.Core;
using Composr.Core.Repositories;
using Composr.Core.Services;
using Composr.Core.Specifications;
using System;
using System.Collections.Generic;

namespace Composr.Services
{
    public class Service<T> :IService<T> where T :class, IComposrEntity
    {
        public Service(IRepository<T> repository, ISpecification<T> specification)
        {
            if (repository == null || specification == null) throw new ArgumentNullException();
            this.repository = repository;
            this.specification = specification;
        }

        public int Count(string criteria)
        {
            return repository.Count(criteria);
        }

        public void Delete(T t)
        {
            if (t == null || !t.Id.HasValue) throw new ArgumentNullException();
            repository.Delete(t);
        }

        public T Get(int id)
        {
            return repository.Get(id);
        }

        public IList<T> Get(Filter filter)
        {
            return repository.Get(filter);
        }

        public int Save(T t)
        {
            if (t == null) throw new ArgumentNullException();
            var compliance = specification.EvaluateCompliance(t);
            if (!compliance.IsSatisfied) throw new SpecificationException(string.Join(Environment.NewLine, compliance.Errors));
            return repository.Save(t);
        }

        protected readonly IRepository<T> repository;
        protected readonly ISpecification<T> specification;
    }
}