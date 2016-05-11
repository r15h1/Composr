using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core.Repositories
{
    public interface IPostRepository:IRepository<Post>
    {
        IList<Post> GetPublishedPosts(Filter filter);
    }
}
