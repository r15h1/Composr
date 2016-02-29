using Composr.Core;
using Composr.Core.Specifications;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Specifications
{
    internal class BlogValidator : FluentValidation.AbstractValidator<Blog>
    {
        public BlogValidator()
        {
            RuleSet("BlogID", () => { RuleFor(blog => blog.Id).Must((blog, blogid) => !blogid.HasValue || (blogid.HasValue && blogid.Value > 0)); });
            RuleSet("Name", () => { RuleFor(blog => blog.Name).NotEmpty().Must(Composr.Util.Extensions.IsAlphaNumericWhiteSpaceUnderscore); });
            RuleSet("Locale", () => { RuleFor(blog => blog.Locale).NotNull(); });
        }
    }

    public class MinimalBlogSpecification : Composr.Core.Specifications.ISpecification<Blog>
    {
        public Compliance EvaluateCompliance(Blog blog)
        {
            if (blog == null) return new Core.Specifications.Compliance(new List<string> { "blog cannot be null" });
            var result = new BlogValidator().Validate(blog, ruleSet: "BlogID,Name,Locale");
            return new Compliance(result.Errors.Select(x => x.ErrorMessage));
        }
    }    
}
