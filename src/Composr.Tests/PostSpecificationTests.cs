using Composr.Core;
using Composr.Core;
using Composr.Specifications;
using Xunit;
using System;

namespace Composr.Tests
{
    
    public class PostSpecificationTests
    {
        private const int URN_MIN_LENGTH = 5;
        private const int URN_MAX_LENGTH = 200;
        private const int META_DESC_MIN_LENGTH = 150;
        private const int META_DESC_MAX_LENGTH = 160;

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
           ISpecification<Post> specification = new PostSpecification();
           var compliance = specification.EvaluateCompliance(null);
           Assert.False(compliance.IsSatisfied);
        }        

        [Fact]
        public void PostWhenNewCanHaveDraftStatus()
        {
            Post post = new Post(new Blog(1)) { Title = "title", Status = PostStatus.DRAFT };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanHavePublishedStatus()
        {
            Post post = new Post(new Blog(1)) { Title = "title", Status = PostStatus.PUBLISHED, URN = "/test/path"};
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 150));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        //[Fact]
        //public void PostWhenNewCanNotHaveDeletedStatus()
        //{
        //    Post post = new Post(new Blog(1)) { Title = "title", Status = PostStatus.DELETED };
        //    ISpecification<Post> specification = new MinimalPostSpecification();
        //    var compliance = specification.EvaluateCompliance(post);
        //    Assert.False(compliance.IsSatisfied);
        //}

        [Fact]
        public void PostWhenNewCanNotHaveNullTitle()
        {
            Post post = new Post(new Blog(1)) { Title =null };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveEmptyTitle()
        {
            Post post = new Post(new Blog(1)) { Title = string.Empty };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveWhitespaceTitle()
        {
            Post post = new Post(new Blog(1)) { Title = "  " };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenNewCanNotHaveValidTitle()
        {
            Post post = new Post(new Blog(1)) { Title = "Post title" };
            ISpecification<Post> specification = new PostSpecification();
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
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanHaveDraftStatus()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.DRAFT };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanHavePublishedStatus()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MIN_LENGTH + 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', META_DESC_MIN_LENGTH + 1));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        //[Fact]
        //public void PostWhenExistingCanNotHaveDeletedStatus()
        //{
        //    Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.DELETED };
        //    ISpecification<Post> specification = new MinimalPostSpecification();
        //    var compliance = specification.EvaluateCompliance(post);
        //    Assert.False(compliance.IsSatisfied);
        //}

        [Fact]
        public void PostWhenExistingCanNotHaveNullTitle()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = null };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanNotHaveEmptyTitle()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = null };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenExistingCanNotHaveValidTitle()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title" };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        //------------------------------------------------------------------------------------
        [Fact]
        public void PostWhenPublishedMustNotHaveNullURN()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveEmptyURN()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = string.Empty };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 2);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveWhiteSpaceURN()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = "  " };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 2);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveURNWithLessThanMinAllowedChars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MIN_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveURNWithMoreThanMaxAllowedChars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH + 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedURNIsValidWithMinLengthPlus1Chars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MIN_LENGTH + 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenPublishedURNIsValidNWithMaxLengthMinus1Chars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', 155));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostURNMustAlwaysSatisfyRegex()
        {
            //whether or not published
            //test for URN regex
            //must be a valid URN - allowed chars alphanumeric , - and /
            //must not have consecutive - or /
            //must not end with - or /
            Assert.True(1 == 2);
        }

        //------------------------------------------------------------------------------------------
        [Fact]
        public void PostWhenPublishedMustNotHaveMissingMetaDesc()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveNullMetaDesc()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, null);
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveEmptyMetaDesc()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, string.Empty);
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveWhiteSpaceMetaDesc()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, "     ");
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveMetaDescWithMinLengthMinus1Chars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', META_DESC_MIN_LENGTH - 1));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedMustNotHaveMetaDescWithMaxLengthPlus1Chars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', META_DESC_MAX_LENGTH + 1));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.Errors.Count == 1);
        }

        [Fact]
        public void PostWhenPublishedCanHaveMetaDescWithMaxLengthMinus1Chars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', META_DESC_MAX_LENGTH - 1));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostWhenPublishedCanHaveMetaDescWithMinLengthPlus1Chars()
        {
            Post post = new Post(new Blog(1)) { Id = 123, Title = "title", Status = PostStatus.PUBLISHED, URN = new string('a', URN_MAX_LENGTH - 1) };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, new string('a', META_DESC_MIN_LENGTH + 1));
            ISpecification<Post> specification = new PostSpecification();
            var compliance = specification.EvaluateCompliance(post);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void PostMetaDescMustAlwaysSatisfyRegex()
        {
            //alpha numeric, punctuation and space
            Assert.True(1 == 2);
        }
    }
} 