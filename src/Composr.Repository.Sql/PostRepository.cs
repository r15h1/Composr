using Composr.Core;
using Composr.Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Repository.Sql
{
    public class PostRepository:Composr.Core.Repositories.IRepository<Post>
    {
        public PostRepository(Blog blog) 
        {
            if (blog == null) throw new ArgumentNullException();
            Blog = blog;
            Locale = blog.Locale.Value;
        }

        public Blog Blog { get; private set; }

        public Core.Locale Locale { get; set; }

        public Core.Post Get(int id)
        {
            return Fetch(id, Locale);
        }

        private Core.Post Fetch(int postid, Core.Locale locale)
        {
            var p = new DynamicParameters();
            p.Add("@LocaleID", (int)locale);
            p.Add("@PostID", postid);

            using (System.Data.IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                return conn.Query("Post_Select_One", p, commandType: System.Data.CommandType.StoredProcedure).Select(
                        row => new Composr.Core.Post(new BlogRepository().Get((int)row.BlogID))
                        {
                            Body = row.Body,
                            ID = row.PostID,
                            Status = (Core.PostStatus)Enum.Parse(typeof(Core.PostStatus), row.PostStatusID.ToString()),
                            Title = row.Title
                        }                        
                ).SingleOrDefault<Post>() ;
            }
        }

        public IList<Core.Post> Get(Filter filter)
        {
            if (filter == null) filter = new Filter();
            return Fetch(filter.Criteria, Locale, filter.Offset, filter.Limit);
        }

        private IList<Post> Fetch(string criteria, Locale locale, int? offset, int? limit)
        {
            var p = new DynamicParameters();
            p.Add("@Criteria", criteria);
            p.Add("@LocaleID", (int)locale);

            if (offset.HasValue) p.Add("@Offset", offset.Value);
            if (limit.HasValue) p.Add("@Limit", limit.Value);

            using (System.Data.IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                return conn.Query("Post_Select_Many", p, commandType: System.Data.CommandType.StoredProcedure).Select(
                        row => new Composr.Core.Post(new BlogRepository().Get((int)row.BlogID))
                        {
                            Body = row.Body,
                            ID = row.PostID,
                            Status = (Core.PostStatus)Enum.Parse(typeof(Core.PostStatus), row.PostStatusID.ToString()),
                            Title = row.Title
                        }
                ).ToList<Post>();
            }
        }

        public int Count(string criteria)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", Blog.ID);
            p.Add("@Criteria", criteria);
            p.Add("@LocaleID", (int)Locale);
            return QueryExecutor.ExecuteSingle<int>("Post_Count", p);
        }

        public int Save(Core.Post post)
        {
            if (!post.ID.HasValue) return Create(post);
            Update(post);
            return post.ID.Value;
        }

        private void Update(Post post)
        {
            var p = new DynamicParameters();
            p.Add("@PostID", post.ID);
            p.Add("@LocaleID", (int)post.Blog.Locale);
            p.Add("@Title", post.Title);
            p.Add("@Body", post.Body);
            p.Add("@PostStatusID", (int)post.Status);

            QueryExecutor.ExecuteSingle<Post>("Post_Update", p);
        }

        /// <summary>
        /// inserts a new blog record in the db
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        private int Create(Core.Post post)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", post.Blog.ID);
            p.Add("@LocaleID", (int)post.Blog.Locale);
            p.Add("@Title", post.Title);
            p.Add("@Body", post.Body);
            p.Add("@PostStatusID", (int)post.Status);

            return QueryExecutor.ExecuteSingle<int>("Post_Create", p);
        }

        public void Delete(Core.Post post)
        {
            var p = new DynamicParameters();
            p.Add("@PostID", post.ID);
            p.Add("@LocaleID", (int)post.Blog.Locale);

            QueryExecutor.ExecuteSingle<Post>("Post_Delete", p);
        }
    }
}
