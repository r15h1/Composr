using Composr.Core;
using Composr.Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Composr.Repository.Sql
{
    public class PostRepository:Composr.Core.Repositories.IPostRepository
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
            Core.Post post = null;
            var p = new DynamicParameters();
            p.Add("@LocaleID", (int)locale);
            p.Add("@PostID", postid);

            using (System.Data.IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                //return conn.Query("Post_Select_One", p, commandType: System.Data.CommandType.StoredProcedure).Select<dynamic, Post>(
                //        row => BuildPost(row)
                //).SingleOrDefault();


                using (var reader = conn.QueryMultiple("Post_Select_One", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    post = reader.Read().Select<dynamic, Post>(row => BuildPost(row)).SingleOrDefault();
                    if (post != null)
                        post.Attributes = reader.Read().Select<dynamic, KeyValuePair<string, string>>(row => BuildAttributes(row)).ToDictionary(x => x.Key, x=>x.Value);
                };
                return post;
            }
        }

        private KeyValuePair<string, string> BuildAttributes(dynamic row)
        {
            return new KeyValuePair<string, string>(row.Key, row.Value);
        }

        private Post BuildPost(dynamic row)
        {
            return new Composr.Core.Post(new BlogRepository().Get((int)row.BlogID))
            {
                Body = row.Body,
                Id = row.PostID,
                Status = (Core.PostStatus)Enum.Parse(typeof(Core.PostStatus), row.PostStatusID.ToString()),
                Title = row.Title,
                URN = row.URN,
                DatePublished = row.DatePublished,
                DateCreated = row.DateCreated
            };
        }

        public IList<Core.Post> Get(Filter filter)
        {
            if (filter == null) filter = new Filter();
            return Fetch("Post_Select_Many", filter.Criteria, Locale, filter.Offset, filter.Limit);
        }

        private IList<Post> Fetch(string command, string criteria, Locale locale, int? offset, int? limit)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", Blog.Id);
            p.Add("@Criteria", criteria);
            p.Add("@LocaleID", (int)locale);

            if (offset.HasValue) p.Add("@Offset", offset.Value);
            if (limit.HasValue) p.Add("@Limit", limit.Value);

            using (System.Data.IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                return conn.Query(command, p, commandType: System.Data.CommandType.StoredProcedure).Select<dynamic, Post>(
                        row => BuildPost(row)
                ).ToList();
            }
        }

        public int Count(string criteria)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", Blog.Id);
            p.Add("@Criteria", criteria);
            p.Add("@LocaleID", (int)Locale);
            return QueryExecutor.ExecuteSingle<int>("Post_Count", p);
        }

        public int Save(Core.Post post)
        {
            if (!post.Id.HasValue)
                return Create(post);

            Update(post);
            return post.Id.Value;
        }

        private void Update(Post post)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", post.Blog.Id);
            p.Add("@PostID", post.Id);
            p.Add("@LocaleID", (int)post.Blog.Locale);
            p.Add("@Title", post.Title);
            p.Add("@Body", post.Body);
            p.Add("@PostStatusID", (int)post.Status);
            p.Add("@URN", post.URN);
            p.Add("@Attributes", GetPostAttributesDataTable(post.Attributes).AsTableValuedParameter("dbo.PostAttrributeTableType"));
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
            p.Add("@BlogID", post.Blog.Id);
            p.Add("@LocaleID", (int)post.Blog.Locale);
            p.Add("@Title", post.Title);
            p.Add("@Body", post.Body);
            p.Add("@PostStatusID", (int)post.Status);
            p.Add("@URN", post.URN);
            p.Add("@Attributes", GetPostAttributesDataTable(post.Attributes).AsTableValuedParameter("dbo.PostAttrributeTableType"));
            return QueryExecutor.ExecuteSingle<int>("Post_Create", p);
        }

        private DataTable GetPostAttributesDataTable(IDictionary<string, string> attributes)
        {
            var dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            if(attributes != null && attributes.Count > 0)
                foreach(var attr in attributes)
                    dt.Rows.Add(attr.Key, attr.Value);                
            
            return dt;
        }

        public void Delete(Core.Post post)
        {
            var p = new DynamicParameters();
            p.Add("@PostID", post.Id);
            p.Add("@LocaleID", (int)post.Blog.Locale);

            QueryExecutor.ExecuteSingle<Post>("Post_Delete", p);
        }

        public IList<Post> GetPublishedPosts(Filter filter)
        {
            if (filter == null) filter = new Filter();
            return Fetch("Post_Select_Published", filter.Criteria, Locale, filter.Offset, filter.Limit);
        }
    }
}