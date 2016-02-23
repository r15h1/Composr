using System;
using Xunit;
using System.Transactions;
using Composr.Core;
using Composr.Repository.Sql;

namespace Composr.Tests
{
    
    public class PostServiceTests
    {
        [Fact]
        public void PostServiceThrowsSpecificationExceptionIfNullPostIsSaved()
        {
            Composr.Services.PostService service = CreatePostService();                
            Assert.Throws<ArgumentNullException>(() => service.Save(null));
        }

        [Fact]
        public void PostServiceThrowsArgumentNullExceptionWhenSavingPostWithNoBlog()
        {
            Composr.Services.PostService service = CreatePostService();
            Assert.Throws<ArgumentNullException>(() => service.Save(new Post(null)));
         }

        [Fact]
        public void PostServiceThrowsArgumentExceptionWhenSavingPostWithEmptyBlogID()
        {
           Composr.Services.PostService service = CreatePostService();                
           Assert.Throws<ArgumentException>(() => service.Save(new Post(new Blog())));            
        }

        [Fact]
        public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithNullTitle()
        {
            Composr.Services.PostService service = CreatePostService();
            Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))));
        }

        [Fact]
        public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithEmptyTitle()
        {
            Composr.Services.PostService service = CreatePostService();
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
            Composr.Services.PostService service = CreatePostService();                
            Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))
                {
                    Title = "    ",
                    Status = PostStatus.DRAFT
                }
            ));
        }

        [Fact]
        public void PostServiceThrowsSpecificationExceptionWhenSavingPostWithDeletedStatus()
        {
           Composr.Services.PostService service = CreatePostService();
            Assert.Throws<SpecificationException>(() => service.Save(new Post(new Blog(1))
            {
                Title = "abcd",
                Status = PostStatus.DELETED
            }));
        }

        [Fact]
        public void PostServiceSavesPostWithValidTitleAndDraftStatus()
        {
            using(TransactionScope t = new TransactionScope())
            {
                Composr.Services.PostService service = CreatePostService();
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
                Composr.Services.PostService service = CreatePostService();
                int id = service.Save(new Post(new Blog(1))
                {
                    Title = "Abc",
                    Status = PostStatus.PUBLISHED
                }
                );
                Assert.True(id > 0);
            }
        }



        private static Services.PostService CreatePostService()
        {
            Blog blog = new Blog(1);
            Composr.Services.PostService service = new Composr.Services.PostService(new PostRepository(blog));
            return service;
        }
    }
}
