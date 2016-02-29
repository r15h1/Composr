using Composr.Core;
using Composr.Core.Repositories;
using Dapper;
using System.Collections.Generic;

namespace Composr.Repository.Sql
{
    public class BlogRepository : Composr.Core.Repositories.IRepository<Blog>
    {

        public BlogRepository()
        {
            Locale = Locale.EN;
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
            if (!blog.Id.HasValue) return Create(blog);
            
            Update(blog);
            return blog.Id.Value;
        }

        public void Delete(Blog blog)
        {
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
            var p = new DynamicParameters();
            p.Add("@LocaleID", (int)locale);
            p.Add("@BlogID", blogid);
            return QueryExecutor.ExecuteSingle<Blog>("Blog_Select_One", p);
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

            return QueryExecutor.ExecuteList<Blog>("Blog_Select_Many", p);
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