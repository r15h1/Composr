using Composr.Core;
using Composr.Core.Specifications;
using Composr.Specifications;
using Xunit;
using System;

namespace Composr.Tests
{
    
    public class PostSpecificationTests
    {
        [Fact]
        public void PostBlogCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Post(null) { Title = "title", Status = PostStatus.DRAFT });
        }

        [Fact]
        public void PostBlogCannotHaveEmptyID()
        {
            Assert.Throws<ArgumentException>(() => new Post(new Blog()) { Title = "title", Status = PostStatus.DRAFT });
        }

        [Fact]
        public void PostBlogCannotHaveNegativeID()
        {
            Assert.Throws<ArgumentException>(() => new Post(new Blog(-1)) { Title = "title", Status = PostStatus.DRAFT });
        }       


        [Fact]
        public void PostWhenNullDoesNotSatisfyNewPostSpecification()
        {
           ISpecification<Post> specification = new MinimalPostSpecification();
           var compliance = specification.EvaluateCompliance(null);
           Assert.False(compliance.IsSatisfied);
        }        

        [Fact]
        public void PostWhenNewCanHaveDraftStatus()
        {
            Post post = new Post(new Blog(1)) { Title = "title", Status = PostStatus.DRAFT };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanHavePublishedStatus()
        {
            Post post = new Post(new Blog(1)) { Title = "title", Status = PostStatus.PUBLISHED };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveDeletedStatus()
        {
            Post post = new Post(new Blog(1)) { Title = "title", Status = PostStatus.DELETED };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveNullTitle()
        {
            Post post = new Post(new Blog(1)) { Title =null };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveEmptyTitle()
        {
            Post post = new Post(new Blog(1)) { Title = string.Empty };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveWhitespaceTitle()
        {
            Post post = new Post(new Blog(1)) { Title = "  " };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveValidTitle()
        {
            Post post = new Post(new Blog(1)) { Title = "Post title" };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        //------------------------------------------------------------------------------------

        [Fact]
        public void PostWhenExistingMustNotHaveNegativeID() => Assert.Throws<ArgumentException>(()=> new Post(new Blog(1)) { Id = -1, Title = "title" });

       
        [Fact]
        public void PostWhenExistingMustHavePositiveID()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title" };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanHaveDraftStatus()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.DRAFT };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanHavePublishedStatus()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanNotHaveDeletedStatus()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.DELETED };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanNotHaveNullTitle()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = null };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanNotHaveEmptyTitle()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = null };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanNotHaveValidTitle()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title" };
            ISpecification<Post> specification = new MinimalPostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }
    }
}