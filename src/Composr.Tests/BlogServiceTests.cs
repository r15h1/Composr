﻿using Composr.Core;
using Composr.Repository.Sql;
using Composr.Services;
using Xunit;
using System;
using System.Transactions;
using Composr.Specifications;

namespace Composr.Tests
{
    
    public class BlogServiceTests
    {
        [Fact]
        public void BlogServiceThrowsArgumentNullExceptionIfNullBlogIsSaved()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Service<Blog> service = GetService();
                Assert.Throws<ArgumentNullException>(()=> service.Save(null));
            }
        }

        [Fact]
        public void BlogServiceCannotSaveNewBlogWithoutName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                try
                {
                    Service<Blog> service = GetService();
                    Blog blog = new Blog();
                    Assert.Throws<SpecificationException>(()=>service.Save(blog));
                }
                catch (Exception ex)
                {
                    Assert.IsType(typeof(SpecificationException), ex);
                }
            }
        }

        [Fact]
        public void BlogServiceCannotSaveNewBlogWithEmptyName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Service<Blog> service = GetService();
                Blog blog = new Blog() { Name = "" };                    
                Assert.Throws< SpecificationException>(()=> service.Save(blog));                
            }
        }

        [Fact]
        public void BlogServiceCannotSaveNewBlogWithWhitespaceName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Service<Blog> service = GetService();
                Blog blog = new Blog() { Name = "   " };
                Assert.Throws<SpecificationException>(() => service.Save(blog));                
            }
        }

        [Fact]
        public void BlogServiceCannotSaveNewBlogWithSpecialCharacterName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Service<Blog> service = GetService();
                Blog blog = new Blog() { Name = "abc&" };
                Assert.Throws<SpecificationException>(() => service.Save(blog));
            }
        }

        [Fact]
        public void BlogServiceCanSaveNewBlogWithProperName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Service<Blog> service = GetService();
                Blog blog = new Blog() {  Name = "abc" };
                int blogid = service.Save(blog);
                Assert.True(blogid > 0 && !blog.ID.HasValue);
            }
        }

        [Fact]
        public void BlogServiceCanSaveExistingBlogWithProperName()
        {
            using (TransactionScope t = new TransactionScope())
            {
                Service<Blog> service = GetService();
                Blog blog = new Blog(1) { Name = "abc" };
                int blogid = service.Save(blog);
                Assert.True(blogid == 1);
            }
        }

        private Service<Blog> GetService()
        {
            return new Service<Blog>(new BlogRepository(), new MinimalBlogSpecification());
        }
    }
}
