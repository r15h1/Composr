using Composr.Core;
using Composr.Lib.Specifications;
using Composr.Lib.StructuredData;
using Composr.Lib.Util;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace Composr.Tests
{
    public class StructuredDataTests
    {

        public StructuredDataTests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true);
            Settings.Config = builder.Build();
        }

        [Fact]
        public void Init()
        {
            Composr.Core.IBlogRepository blogRepo = new Composr.Repository.Sql.BlogRepository(new MinimalBlogSpecification());
            var blog = blogRepo.Get(1);

            Composr.Core.IPostRepository postRepo = new Composr.Repository.Sql.PostRepository(blog, new PostSpecification(), new Composr.Repository.Sql.BlogRepository(new MinimalBlogSpecification()));
            var post = postRepo.Get(34);
            IStructuredDataTranslator translator = new RecipeTranslator();
            post.AcceptTranslator(translator);
            var recipe = translator.Output;
            var json = recipe.ToJsonLD();

            post = postRepo.Get(33);
            post.AcceptTranslator(translator);
            recipe = translator.Output;
            json = recipe.ToJsonLD();

        }
    }
}