using Composr.Core;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Specifications
{
    internal class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleSet("Blog", () => { RuleFor(post => post.Blog).NotNull().Must((post, blog) => post.Blog.ID.HasValue && post.Blog.ID.Value > 0); });
            RuleSet("PostID", () => { RuleFor(post => post.ID).Must((post, postid) => !postid.HasValue || (postid.HasValue && postid.Value > 0)); });
            RuleSet("Active", () => { RuleFor(post => post.Status).NotEmpty().Must((post, status) => status == PostStatus.DRAFT || status == PostStatus.PUBLISHED); });
            RuleSet("Deleted", () => { RuleFor(post => post.Status).NotEmpty().Equal(PostStatus.DELETED); });
            RuleSet("Title", () => { RuleFor(post => post.Title).NotEmpty(); });            
        }        
    }

    public class MinimalPostSpecification : Composr.Core.Specifications.ISpecification<Post>
    {
        public Core.Specifications.Compliance EvaluateCompliance(Post post)
        {
            if (post == null) return new Core.Specifications.Compliance(new List<string> { "post cannot be null" });

            var result = new PostValidator().Validate(post, ruleSet: "Blog,PostID,Active,Title");
            return new Core.Specifications.Compliance(result.Errors.Select(x => x.ErrorMessage));
        }
    }

    //public class ExistingPostSpecification : Composr.Core.Specifications.ISpecification<Post>
    //{
    //    public Core.Specifications.Compliance EvaluateCompliance(Post post)
    //    {
    //        if (post == null) return new Core.Specifications.Compliance(new List<string> { "post cannot be null" });

    //        var result = new PostValidator().Validate(post, ruleSet: "BlogID,ExistingPost,Active,Title");
    //        return new Core.Specifications.Compliance(result.Errors.Select(x => x.ErrorMessage));
    //    }
    //}
}