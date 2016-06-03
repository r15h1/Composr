using Composr.Core;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Lib.Specifications
{
    internal class BlogValidator : FluentValidation.AbstractValidator<Blog>
    {
        public BlogValidator()
        {
            RuleSet("BlogID", () => { RuleFor(blog => blog.Id).Must((blog, blogid) => !blogid.HasValue || (blogid.HasValue && blogid.Value > 0)); });
            RuleSet("Name", () => { RuleFor(blog => blog.Name).NotEmpty().Must(Composr.Lib.Util.Extensions.IsAlphaNumericWhiteSpaceUnderscore); });
            RuleSet("Locale", () => { RuleFor(blog => blog.Locale).NotNull(); });
        }
    }

    public class MinimalBlogSpecification : Composr.Core.ISpecification<Blog>
    {
        public Compliance EvaluateCompliance(Blog blog)
        {
            if (blog == null) return new Compliance(new List<string> { "blog cannot be null" });
            var result = new BlogValidator().Validate(blog, ruleSet: "BlogID,Name,Locale");
            return new Compliance(result.Errors.Select(x => x.ErrorMessage));
        }
    }    
}
