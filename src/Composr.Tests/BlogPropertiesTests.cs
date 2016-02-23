using Xunit;

namespace ComposrTests
{

    public class BlogPropertiesTests
    {
        Composr.Core.Blog blog;
        public BlogPropertiesTests()
        {
            blog = new Composr.Core.Blog();
        }
                

        [Fact]
        public void BlogEntityHasEnglishLocaleByDefault()
        {
            Assert.NotNull(blog.Locale);
            Assert.IsType(typeof(Composr.Core.Locale), blog.Locale);
            Assert.True(blog.Locale == Composr.Core.Locale.EN);
        }
        
    }
}