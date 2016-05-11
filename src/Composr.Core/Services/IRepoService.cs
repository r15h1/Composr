using System.Collections.Generic;
using Composr.Core;
using Composr.Core.Repositories;

namespace Composr.Core.Services
{
    public interface IRepoService<T> where T :class, IComposrEntity
    {
        int Count(string criteria);
        void Delete(T t);
        T Get(int id);
        IList<T> Get(Filter filter);
        int Save(T t);
    }
}