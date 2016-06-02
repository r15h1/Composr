using System;
using Xunit;
using System.Transactions;
using Composr.Core;
using Composr.Repository.Sql;
using Composr.Specifications;

namespace Composr.Tests
{
    
    public class PostServiceTests
    {
        [Fact]
        public void PostServiceThrowsSpecificationExceptionIfNullPostIsSaved()
        {
            Composr.Lib.Services.RepoService<Post> service = CreatePostService();                
            Assert.Throws<ArgumentNullException>(() => service.Save(null));
        }

        [Fact]
        public void PostServiceThrowsArgumentNullExceptionWhenSavingPostWithNoBlog()
        {
            Composr.Lib.Services.RepoService<Post> service = CreatePostService();
            Assert.Throws<ArgumentNullException>(() => service.Save(new Post(null)));
         }

        [Fact]
        public void PostServiceThrowsArgumentExceptionWhenSavingPostWithEmptyBlogID()
        {
           Composr.Lib.Services.RepoService<Post> service = CreatePostService();                
           Assert.Throws<ArgumentException>(() => service.Save(new Post(new Blog())));            
        }

        [Fact]
        public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithNullTitle()
        {
            Composr.Lib.Services.RepoService<Post> service = CreatePostService();
            Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))));
        }

        [Fact]
        public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithEmptyTitle()
        {
            Composr.Lib.Services.RepoService<Post> service = CreatePostService();
            Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))
                    {
                        Title = "",
                        Status = PostStatus.DRAFT
                    }
                ));
        }

        [Fact]
        public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithWhitespaceTitle()
        {
            Composr.Lib.Services.RepoService<Post> service = CreatePostService();                
            Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))
                {
                    Title = "    ",
                    Status = PostStatus.DRAFT
                }
            ));
        }

        //[Fact]
        //public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithDeletedStatus()
        //{
        //   Composr.Lib.Services.Service<Post> service = CreatePostService();
        //    Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))
        //    {
        //        Title = "abcd",
        //        Status = PostStatus.DELETED
        //    }));
        //}

        [Fact]
        public void PostServiceSavesPostWithValidTitleAndDraftStatus()
        {
            using(TransactionScope t = new TransactionScope())
            {
                Composr.Lib.Services.RepoService<Post> service = CreatePostService();
                int id = service.Save(new Post(new Blog(1))
                    {
                        Title = "Abc",
                        Status = PostStatus.DRAFT
                    }
                );
                Assert.True(id > 0);
            }                
        }

        [Fact]
        public void PostServiceSavesPostWithValidTitleAndPublishedStatus()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Composr.Lib.Services.RepoService<Post> service = CreatePostService();
                Post post = new Post(new Blog(1))
                {
                    Title = "Abc",
                    Status = PostStatus.PUBLISHED,
                    URN = new string('a', 5)
                };

                post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
                int id = service.Save(post);
                Assert.True(id > 0);
            }
        }



        private static Lib.Services.RepoService<Post> CreatePostService()
        {
            Blog blog = new Blog(1);
            Composr.Lib.Services.RepoService<Post> service = new Composr.Lib.Services.RepoService<Post>(new PostRepository(blog), new PostSpecification());
            return service;
        }
    }
}
