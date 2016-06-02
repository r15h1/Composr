using System.Collections.Generic;

namespace Composr.Core
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