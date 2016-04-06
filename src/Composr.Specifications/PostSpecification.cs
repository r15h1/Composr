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
            RuleSet("Blog", () => { RuleFor(post => post.Blog).NotNull().Must((post, blog) => post.Blog.Id.HasValue && post.Blog.Id.Value > 0); });
            RuleSet("PostID", () => { RuleFor(post => post.Id).Must((post, postid) => !postid.HasValue || (postid.HasValue && postid.Value > 0)); });
            RuleSet("Active", () => { RuleFor(post => post.Status).NotEmpty().Must((post, status) => status == PostStatus.DRAFT || status == PostStatus.PUBLISHED); });            
            RuleSet("Title", () => { RuleFor(post => post.Title).NotEmpty(); });
            RuleSet("Published", () =>
            {
                When(post => post.Status == PostStatus.PUBLISHED, () =>
                {
                    RuleFor(post => post.URN).NotEmpty().Length(min: 5, max: 200);
                    RuleFor(post => post.Attributes).Cascade(CascadeMode.StopOnFirstFailure)
                        .NotNull()
                        .Must((post, attributes) => attributes.Count > 0 && attributes.ContainsKey(PostAttributeKeys.MetaDescription)).WithMessage("Meta description is missing")
                        .Must((post, attributes) => !string.IsNullOrWhiteSpace(attributes[PostAttributeKeys.MetaDescription]) && attributes[PostAttributeKeys.MetaDescription].Length >= 150 && attributes[PostAttributeKeys.MetaDescription].Length <= 160).WithMessage("Meta description must be between 150 and 160 characters long");
                });
            });
        }        
    }

    public class BasicPostSpecification : Composr.Core.Specifications.ISpecification<Post>
    {
        public Core.Specifications.Compliance EvaluateCompliance(Post post)
        {
            if (post == null) return new Core.Specifications.Compliance(new List<string> { "post cannot be null" });

            var result = new PostValidator().Validate(post, ruleSet: "Blog,PostID,Active,Title");
            return new Core.Specifications.Compliance(result.Errors.Select(x => x.ErrorMessage));
        }
    }

    public class PublishedPostSpecification : Composr.Core.Specifications.ISpecification<Post>
    {
        public Core.Specifications.Compliance EvaluateCompliance(Post post)
        {
            var result = new PostValidator().Validate(post, ruleSet: "Published");
            return new Core.Specifications.Compliance(result.Errors.Select(x => x.ErrorMessage));
        }
    }

}