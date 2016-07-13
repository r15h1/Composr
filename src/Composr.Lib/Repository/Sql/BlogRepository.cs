using Composr.Core;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Repository.Sql
{
    public class BlogRepository : Composr.Core.IBlogRepository
    {
        private ISpecification<Blog> specification;

        public BlogRepository(ISpecification<Blog> specification)
        {
            if (specification == null) throw new ArgumentNullException();
            Locale = Locale.EN;
            this.specification = specification;
        }

        public Locale Locale { get; set; }

        public Blog Get(int id)
        {
            return Fetch(id, Locale);
        }

        public IList<Blog> Get(Filter filter)
        {
            if (filter == null) filter = new Filter();
            return Fetch(filter.Criteria, Locale, filter.Offset, filter.Limit);
        }

        /// <summary>
        /// if blogid is null, then creates a new blog, otherwise updates an existing one
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        public int Save(Blog blog)
        {
            Validate(blog);
            if (!blog.Id.HasValue)
                return Create(blog);

            Update(blog);
            return blog.Id.Value;
        }

        private void Validate(Blog blog)
        {
            if (blog == null) throw new ArgumentNullException();
            var compliance = specification.EvaluateCompliance(blog);
            if (!compliance.IsSatisfied) throw new SpecificationException(string.Join(Environment.NewLine, compliance.Errors));
        }

        public void Delete(Blog blog)
        {
            if (blog == null || !blog.Id.HasValue) throw new ArgumentNullException();
            MarkDeleted(blog);
        }

        public int Count(string criteria)
        {
            return Count(criteria, Locale);
        }

        private int Count(string criteria, Core.Locale locale)
        {
            var p = new DynamicParameters();
            p.Add("@Criteria", criteria);
            p.Add("@LocaleID", (int)locale);
            return QueryExecutor.ExecuteSingle<int>("Blog_Count", p);
        }

        /// <summary>
        /// inserts a new blog record in the db
        /// </summary>
        /// <param name="blog"></param>
        /// <returns></returns>
        private int Create(Blog blog)
        {
            var p = new DynamicParameters();
            p.Add("@LocaleID", (int)blog.Locale);
            p.Add("@Name", blog.Name);
            p.Add("@Description", blog.Description);
            p.Add("@URL", blog.Url);
            return QueryExecutor.ExecuteSingle<int>("Blog_Create", p);
        }

        /// <summary>
        /// gets the blog matching the id and locale
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        private Blog Fetch(int blogid, Locale locale)
        {
            //string sql = "SELECT [BlogID], [LocaleID] as Locale, [Name], [Description], [URL] FROM Blogs WHERE BlogID = @BlogId AND LocaleID = @LocaleID AND ISNULL(Deleted, 0) = 0";
            Blog blog = null;
            var p = new DynamicParameters();
            p.Add("@LocaleID", (int)locale);
            p.Add("@BlogID", blogid);

            using (System.Data.IDbConnection conn = ConnectionFactory.CreateConnection())
            {
                //return conn.Query("Post_Select_One", p, commandType: System.Data.CommandType.StoredProcedure).Select<dynamic, Post>(
                //        row => BuildPost(row)
                //).SingleOrDefault();


                using (var reader = conn.QueryMultiple("Blog_Select_One", p, commandType: System.Data.CommandType.StoredProcedure))
                {
                    blog = reader.Read().Select<dynamic, Blog>(row => BuildBlog(row)).SingleOrDefault();
                    if(blog != null)
                        blog.Attributes = reader.Read().Select<dynamic, KeyValuePair<string, string>>(row => BuildAttributes(row)).ToDictionary(x => x.Key, x => x.Value);
                };
                return blog;
            }

            //return QueryExecutor.ExecuteSingle<Blog>("Blog_Select_One", p);
        }

        private Blog BuildBlog(dynamic row)
        {
            return new Composr.Core.Blog((int)row.BlogID)
            {
                Description = row.Description,
                Locale = (Core.Locale)Enum.Parse(typeof(Core.Locale), row.Locale.ToString()),
                Name = row.Name,
                Url = row.URL
            };
        }
        private KeyValuePair<string, string> BuildAttributes(dynamic row)
        {
            return new KeyValuePair<string, string>(row.Key, row.Value);
        }

        /// <summary>
        /// gets the blog matching the id and locale
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        private IList<Blog> Fetch(string criteria, Locale locale, int? offset, int? limit)
        {
            var p = new DynamicParameters();
            p.Add("@Criteria", criteria);
            p.Add("@LocaleID", (int)locale);

            if (offset.HasValue) p.Add("@Offset", offset.Value);
            if (limit.HasValue) p.Add("@Limit", limit.Value);

            //return QueryExecutor.ExecuteList<Blog>("Blog_Select_Many", p);
            using (System.Data.IDbConnection conn = ConnectionFactory.CreateConnection())
            using (var reader = conn.QueryMultiple("Blog_Select_Many", p, commandType: System.Data.CommandType.StoredProcedure))
            {
                var blogs = reader.Read<Blog>().ToList();
                var attributes = reader.Read<Attribute>().ToList();

                var attrById = attributes.ToLookup(t => t.Id);

                foreach (var blog in blogs)
                {
                    blog.Attributes = attrById[blog.Id].ToDictionary(k => k.Key, v => v.Value);
                }

                return blogs;
            }
        }
        
        private void Update(Blog blog)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", blog.Id);
            p.Add("@LocaleID", (int)blog.Locale);
            p.Add("@Name", blog.Name);
            p.Add("@Description", blog.Description);
            p.Add("@URL", blog.Url);

            QueryExecutor.ExecuteSingle<Blog>("Blog_Update", p);
        }

        private void MarkDeleted(Blog blog)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", blog.Id);
            p.Add("@LocaleID", (int)blog.Locale);
            QueryExecutor.ExecuteSingle<Blog>("Blog_Delete", p);
        }        
    }
}