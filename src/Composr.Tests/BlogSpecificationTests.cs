using Composr.Core;
using Composr.Core;
using Composr.Specifications;
using Xunit;
using System;

namespace Composr.Tests
{
    
    public class BlogSpecificationTests
    {
        [Fact]
        public void BlogWhenNullDoesNotSatisfyMinimalBlogSpecification()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = null;
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveNullID()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Locale = Locale.EN, Id = null };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveNonNullName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = null };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveNonEmptyName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = string.Empty };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveNonWhitespaceName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "  " };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveNoSpecialCharacterInName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "ab%" };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveNameConsistingOnlyOdAlphaNumericCharactersWhitespaceAndUnderscore()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ " };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewMustHaveLocaleSet()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Locale = null };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenNewWorksWithSetLocale()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Locale = Locale.EN };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.True(compliance.IsSatisfied);
        }

        //---------------------------------------------------Existing---------------------------------------------------------------------------

        [Fact]
        public void BlogWhenExistingMustHaveValidID()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Locale = Locale.EN, Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingMustNotHaveNegativeID() => Assert.Throws<ArgumentException>(() => { new Blog(-1) { Name = "abcfeg XYZ 1234567890_ ", Locale = Locale.EN }; });


        [Fact]
        public void BlogWhenExistingMustHaveNonNullName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = null, Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingMustHaveNonEmptyName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = string.Empty, Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingMustHaveNonWhitespaceName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "  ", Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingMustHaveNoSpecialCharacterInName()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "ab%", Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingMustHaveNameConsistingOnlyOdAlphaNumericCharactersWhitespaceAndUnderscore()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.True(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingMustHaveLocaleSet()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Locale = null, Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.False(compliance.IsSatisfied);
        }

        [Fact]
        public void BlogWhenExistingWorksWithSetLocale()
        {
            ISpecification<Blog> specification = new MinimalBlogSpecification();
            Blog blog = new Blog() { Name = "abcfeg XYZ 1234567890_ ", Locale = Locale.EN, Id = 12 };
            var compliance = specification.EvaluateCompliance(blog);
            Assert.True(compliance.IsSatisfied);
        }

    }
}
