using Composr.Core;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Composr.Repository.Sql
{
    public class BlogRepository : IBlogRepository
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

            using (IDbConnection conn = ConnectionFactory.CreateConnection())
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
            using (IDbConnection conn = ConnectionFactory.CreateConnection())
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

        public ISet<string> GetStopWords(Blog blog)
        {
            var p = new DynamicParameters();
            p.Add("@BlogID", blog.Id);
            p.Add("@LocaleID", (int)blog.Locale);
            return new HashSet<string>(QueryExecutor.ExecuteList<string>("StopWords_Select", p));
        }

        public IDictionary<string, IList<string>> GetSynonyms(Blog blog)
        {
            Dictionary<string, IList<string>> synonyms = new Dictionary<string, IList<string>>();
            var p = new DynamicParameters();
            p.Add("@BlogID", blog.Id);
            p.Add("@LocaleID", (int)blog.Locale);

            using (var conn = ConnectionFactory.CreateConnection())            
            {
                conn.Query("Synonyms_Select", p, commandType: System.Data.CommandType.StoredProcedure)
                                .Select(x => new KeyValuePair<string, string>(x.Word1, x.Word2))
                                .ToList().ForEach((KeyValuePair < string, string > item) => AddToSynonyms(synonyms, item));                
            }
            return synonyms;
        }

        private void AddToSynonyms(Dictionary<string, IList<string>> synonyms, KeyValuePair<string, string> item)
        {
            AddToSynonyms(synonyms, item.Key, item.Value);
            AddToSynonyms(synonyms, item.Value, item.Key);
        }

        private void AddToSynonyms(Dictionary<string, IList<string>> synonyms, string key, string value)
        {
            if (synonyms.ContainsKey(key)) synonyms[key].Add(value);
            else synonyms.Add(key, new List<string> { value });
        }

        public IList<Category> GetCategoryPages(Blog blog)
        {
            Dictionary<string, string> synonyms = new Dictionary<string, string>();
            var p = new DynamicParameters();
            p.Add("@BlogID", blog.Id);
            p.Add("@LocaleID", (int)blog.Locale);

            using (var conn = ConnectionFactory.CreateConnection())
            using (var reader = conn.QueryMultiple("Category_Pages_Select", p, commandType: System.Data.CommandType.StoredProcedure))
            {
                var categories = reader.Read().Select<dynamic, Category>(row => BuildCategory(blog, row)).ToList();
                var translations = reader.Read().Select<dynamic, Category>(row => BuildCategory(null, row)).ToList();
                categories.ForEach((category) => category.Translations = translations.Where(t => t.Id == category.Id));
                return categories;
            }
        }

        private Category BuildCategory(Blog blog, dynamic row)
        {
            if (blog == null)
                return new Category(new Blog(row.BlogId) { Locale = (Locale)row.Locale }) { Id = row.Id, Title = row.Title, URN = row.URN };

            return new Category(blog) { Id = row.Id, Title = row.Title, URN = row.URN };
        }
    }
}