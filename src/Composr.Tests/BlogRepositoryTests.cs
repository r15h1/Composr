using Composr.Core;
using FizzWare.NBuilder;
using Xunit;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;

namespace Composr.Tests
{

    public class BlogRepositoryTests
    {
        private Composr.Core.IBlogRepository repo;

        public BlogRepositoryTests()
        {
            repo = new Composr.Repository.Sql.BlogRepository();
        }

        private Blog CreateNewBlog()
        {
            return new Blog
            {
                Description = "A journey through code",
                Locale = Core.Locale.EN,
                Name = "Code-Labs",
                Url = "http://www.code-labs.ca"
            };
        }

        [Fact]
        public void BlogRepositoryCreatesBlog()
        {
            using (TransactionScope t = new TransactionScope())
            {
                int id = repo.Save(CreateNewBlog());
                Assert.True(id > 0);
            }
        }

        [Fact]
        public void BlogRepositoryDoesNotSaveBlogWithoutValidName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Assert.Throws<System.Data.SqlClient.SqlException>(() => repo.Save(new Blog()));                
            }
        }

        [Fact]
        public void BlogRepositoryUpdatesBlog()
        {
            string desc = "updated blog description";
            string name = "update name";
            string url = "http://updatedurl.net";

            using (TransactionScope t = new TransactionScope())
            {
                int id = repo.Save(CreateNewBlog());
                Blog b = repo.Get(id);
                b.Description = desc;
                b.Name = name;
                b.Url = url;
                int id1 = repo.Save(b);
                Blog b1 = repo.Get(id1);

                Assert.Equal(b1.Id, b.Id);
                Assert.Equal(b1.Locale, b.Locale);
                Assert.Equal(b1.Description, desc);
                Assert.Equal(b1.Name, name);
                Assert.Equal(b1.Url, url);
            }
        }

        [Fact]
        public void BlogRepositoryCanDeleteBlog()
        {           
            using (TransactionScope t = new TransactionScope())
            {
                int id = repo.Save(CreateNewBlog());
                Blog b = repo.Get(id);
                Assert.NotNull(b);

                repo.Delete(b);
                repo.Locale = b.Locale.Value;
                Blog b2 = repo.Get(b.Id.Value);
                Assert.Null(b2);
            }
        }

        [Fact]
        public void BlogRepositoryGetsBlogListOfSameLocale()
        {
            int existing = GetExistingRepoCount();
            int newblogs = 5;

            IList<Blog> list = Builder<Blog>.CreateListOfSize(newblogs)
                    .WhereTheFirst(3).Has(x =>x.Locale = Locale.EN)
                    .AndTheNext(2).Have(x => x.Locale = Locale.FR)
                    .WhereAll().Have(x => x.Url = ("http://" + FizzWare.NBuilder.Generators.GetRandom.WwwUrl())).And(x => x.Id = null)                    
                    .Build();

            using (TransactionScope t = new TransactionScope())
            {
                foreach (Blog b in list) repo.Save(b);
                repo.Locale = Locale.EN;
                IList<Blog> blogs = repo.Get(null);
                Assert.True(blogs.Count == 3 + existing);

                repo.Locale = Locale.FR;
                blogs = repo.Get(null);
                Assert.True(blogs.Count == 2);
            }
        }

        [Fact]
        public void BlogRepositoryGetsBlogsMatchingName()
        {
            IList<Blog> list = Builder<Blog>.CreateListOfSize(5)
                    .WhereTheFirst(1).Has(x => x.Name = "code labs")
                    .AndTheNext(1).Has(x => x.Name = "decode")
                    .AndTheNext(1).Has(x => x.Name = "das hobbit")
                    .AndTheNext(1).Has(x => x.Name = "harry potter")
                    .AndTheNext(1).Has(x => x.Name = "lord of the rings")
                    .WhereAll()
                        .Have(x => x.Description = null)
                        .And(x => x.Id = null)
                        .And(x=> x.Locale = Locale.EN)
                    .Build();

            using (TransactionScope t = new TransactionScope())
            {
                foreach (Blog b in list) repo.Save(b);
                repo.Locale = Locale.EN;
                IList<Blog> blogs = repo.Get(new Composr.Core.Filter { Criteria = "code" });
                Assert.True(blogs.Count == 2);
            }
        }

        [Fact]
        public void BlogRepositoryGetsBlogsMatchingDescription()
        {
            IList<Blog> list = Builder<Blog>.CreateListOfSize(5)
                    .WhereTheFirst(1).Has(x => x.Description = "autotrader")
                    .AndTheNext(1).Has(x => x.Description = "das auto")
                    .AndTheNext(1).Has(x => x.Description = "motoautomatic")
                    .AndTheNext(1).Has(x => x.Description = "man utd")
                    .AndTheNext(1).Has(x => x.Description = "maple leafs")
                    .WhereAll()
                        .Have(x => x.Name = "codelabs")
                        .And(x => x.Id = null)
                        .And(x => x.Locale = Locale.EN)
                    .Build();

            using (TransactionScope t = new TransactionScope())
            {
                foreach (Blog b in list) repo.Save(b);
                repo.Locale = Locale.EN;
                IList<Blog> blogs = repo.Get(new Composr.Core.Filter { Criteria = "auto" });
                Assert.True(blogs.Count == 3);
            }
        }

        [Fact]
        public void BlogRepositoryGetsBlogsMatchingNameOrDescription()
        {
            IList<Blog> list = Builder<Blog>.CreateListOfSize(5)
                    .WhereTheFirst(1).Has(x => x.Description = "new york fries").And(x => x.Name = "hot fries")
                    .AndTheNext(1).Has(x => x.Description = "poutine").And(x => x.Name = "poutinos")
                    .AndTheNext(1).Has(x => x.Description = "yorkdale mall downtown").And(x => x.Name = "yorkdale mall")
                    .AndTheNext(1).Has(x => x.Description = "manchester untied football club").And(x => x.Name = "manutd.com")
                    .AndTheNext(1).Has(x => x.Description = "toronto raptors basket ball").And(x => x.Name = "raptors")
                    .WhereAll()
                        .Have(x => x.Id = null)
                        .And(x => x.Locale = Locale.EN)
                    .Build();

            using (TransactionScope t = new TransactionScope())
            {
                foreach (Blog b in list) repo.Save(b);
                repo.Locale = Locale.EN;
                IList<Blog> blogs = repo.Get(new Composr.Core.Filter { Criteria = "york" });
                Assert.True(blogs.Count == 2);
            }
        }

        [Fact]
        public void BlogRepositoryDoesNotGetDeletedBlogs()
        {
            int existing = GetExistingRepoCount();
            int newblogs = 5;

            IList<Blog> list = Builder<Blog>.CreateListOfSize(newblogs)
                    .WhereTheFirst(1).Has(x => x.Description = "new york fries").And(x => x.Name = "hot fries")
                    .AndTheNext(1).Has(x => x.Description = "poutine").And(x => x.Name = "poutinos")
                    .AndTheNext(1).Has(x => x.Description = "yorkdale mall downtown").And(x => x.Name = "yorkdale mall")
                    .AndTheNext(1).Has(x => x.Description = "manchester untied football club").And(x => x.Name = "manutd.com")
                    .AndTheNext(1).Has(x => x.Description = "toronto raptors basket ball").And(x => x.Name = "raptors")
                    .WhereAll()
                        .Have(x => x.Id = null)
                        .And(x => x.Locale = Locale.EN)
                    .Build();

            using (TransactionScope t = new TransactionScope())
            {
                repo.Locale = Locale.EN;
                foreach (Blog b in list) repo.Save(b);
                IList<Blog> blogs = repo.Get(null);
                Assert.True(blogs.Count == newblogs + existing);
                Blog blogtodelete = blogs.Single(x => x.Description == "new york fries");
                repo.Delete(blogtodelete);
                blogs = repo.Get(new Composr.Core.Filter{ Criteria= "york" });
                Assert.True(blogs.Count == 1);
                blogs = repo.Get(null);
                Assert.True(blogs.Count == 4 + existing);
            }
        }
        
        [Fact]
        public void BlogRepositoryIsSQLInjectionSafe()
        {
            IList<Blog> list = Builder<Blog>.CreateListOfSize(5)
                    .WhereTheFirst(1).Has(x => x.Description = "new york fries").And(x => x.Name = "hot fries")
                    .AndTheNext(1).Has(x => x.Description = "poutine").And(x => x.Name = "poutinos")
                    .AndTheNext(1).Has(x => x.Description = "yorkdale mall downtown").And(x => x.Name = "yorkdale mall")
                    .AndTheNext(1).Has(x => x.Description = "manchester untied football club").And(x => x.Name = "manutd.com")
                    .AndTheNext(1).Has(x => x.Description = "toronto raptors basket ball").And(x => x.Name = "raptors")
                    .WhereAll()
                        .Have(x => x.Id = null)
                        .And(x => x.Locale = Locale.EN)
                    .Build();
            
            IList<Blog> blogs = null;
            using (TransactionScope t = new TransactionScope())
            {
                repo.Locale = Locale.EN;
                foreach (Blog b in list) repo.Save(b);
                blogs = repo.Get(new Composr.Core.Filter { Criteria = "*" });
                Assert.True(blogs.Count == 0);
                blogs = repo.Get(new Composr.Core.Filter { Criteria = "'york' OR 1=1" });
                Assert.True(blogs.Count == 0);
            }

            blogs = repo.Get(new Composr.Core.Filter { Criteria = "york; DROP TABLE Locales;" });
            Assert.True(blogs.Count == 0);
        }

        [Fact]
        public void BlogRepositorySupportsPagination()
        {
            int existing = GetExistingRepoCount();
            int newblogs = 5;

            IList<Blog> list = Builder<Blog>.CreateListOfSize(newblogs)
                     .WhereTheFirst(1).Has(x => x.Description = "new york fries").And(x => x.Name = "hot fries")
                     .AndTheNext(1).Has(x => x.Description = "newton apple").And(x => x.Name = "gravitation")
                     .AndTheNext(1).Has(x => x.Description = "news channel").And(x => x.Name = " cbc news")
                     .AndTheNext(1).Has(x => x.Description = "new kid on the block").And(x => x.Name = "new kids")
                     .AndTheNext(1).Has(x => x.Description = "toronto raptors basket ball").And(x => x.Name = "raptors")
                     .WhereAll()
                         .Have(x => x.Id = null)
                         .And(x => x.Locale = Locale.EN)
                     .Build();

            IList<Blog> blogs;
            using (TransactionScope t = new TransactionScope())
            {
                int count = 0;
                repo.Locale = Locale.EN;
                foreach (Blog b in list) repo.Save(b);
                count = repo.Count(null);
                Assert.True(count == 5 + existing);

                blogs = repo.Get(new Composr.Core.Filter { Criteria = "new", Limit = 2, Offset = 2 });
                
                Assert.True(blogs.Count == 2);
                Assert.True(blogs[0].Name == "hot fries");
                Assert.True(blogs[1].Name == "new kids");
            }
        }

        [Fact]
        public void BlogRepositoryCanCount()
        {
            int existing = GetExistingRepoCount();
            int newblogs = 5;

            IList<Blog> list = Builder<Blog>.CreateListOfSize(newblogs)
                     .WhereTheFirst(1).Has(x => x.Description = "new york fries").And(x => x.Name = "hot fries")
                     .AndTheNext(1).Has(x => x.Description = "poutine").And(x => x.Name = "poutinos")
                     .AndTheNext(1).Has(x => x.Description = "yorkdale mall downtown").And(x => x.Name = "yorkdale mall")
                     .AndTheNext(1).Has(x => x.Description = "manchester untied football club").And(x => x.Name = "manutd.com")
                     .AndTheNext(1).Has(x => x.Description = "toronto raptors basket ball").And(x => x.Name = "raptors")
                     .WhereAll()
                         .Have(x => x.Id = null)
                         .And(x => x.Locale = Locale.EN)
                     .Build();

            using (TransactionScope t = new TransactionScope())
            {
                int count = 0;
                repo.Locale = Locale.EN;
                foreach (Blog b in list) repo.Save(b);
                count = repo.Count(null);
                Assert.True(count == newblogs + existing);
                
                count = repo.Count("york");
                Assert.True(count == 2);               
            }
        }

        private int GetExistingRepoCount()
        {
            return repo.Count(null);
        }

    }
}
