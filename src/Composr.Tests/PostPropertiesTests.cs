using System;
using Xunit;
using Composr.Core;

namespace Composr.Tests
{
    
    public class PostPropertiesTests
    {
        Composr.Core.Post post;

        public PostPropertiesTests()
        {
            post = new Post(new Composr.Core.Blog(1));
        }
              
        [Fact]
        public void PostEntityHasDraftStatusByDefault()
        {
            Assert.True(post.Status == Composr.Core.PostStatus.DRAFT);
        }

        [Fact]
        public void PostMustNotBeInstantiatedWithNullBlog() => Assert.Throws<ArgumentNullException>(() => { new Post(null); });

        [Fact]
        public void PostMustNotBeInstantiatedWithBlogHavingNoID() => Assert.Throws<ArgumentException>(()=> { new Post(new Blog()); });
        
        [Fact]
        public void PostMustNotBeInstantiatedWithBlogHavingNegativeID() => Assert.Throws<ArgumentException>(()=> { new Post(new Blog(-1)); });

        [Fact]
        public void PostMustBeInstantiatedWithValidBlog()
        {
            Post p = new Post(new Blog(1));
            Assert.NotNull(p);
        }
    }
}
