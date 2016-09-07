using System.Collections.Generic;

namespace Composr.Core
{
    public interface IPostRepository:IRepository<Post>
    {
        IList<Post> GetPublishedPosts(Filter filter);

        IList<Post> GetTranslatedPosts(int postid);

        void Translate(int postid, Locale locale);
    }
}
