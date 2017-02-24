using Composr.Core;
using Composr.Lib.Util;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Composr.Lib.StructuredData
{
    public class RecipeTranslator : IStructuredDataTranslator
    {
        private Recipe recipe;
        public IStructuredData Output { get { return recipe; } }

        public void Translate(Post post)
        {
            var trimmedChars = new char[] { '/' };

            recipe = new Recipe()
            {
                Name = post.Title,
                Description = post.Attributes[PostAttributeKeys.MetaDescription],                
                Author = post.Blog.Name
            };

            if (post.DatePublished.HasValue) recipe.DatePublished = post.DatePublished.Value.ToString("yyyy-MM-dd");
            if (    post.Images != null && post.Images.Count > 0 && !post.Blog.Url.IsBlank() 
                    && post.Blog.Attributes !=null && post.Blog.Attributes.ContainsKey(BlogAttributeKeys.ImageLocation)
                    && !post.Blog.Attributes[BlogAttributeKeys.ImageLocation].IsBlank()
               )
                recipe.Image = $"{post.Blog.Url.TrimEnd(trimmedChars)}/{post.Blog.Attributes[BlogAttributeKeys.ImageLocation].TrimEnd(trimmedChars)}/{Regex.Replace(post.Images[0].Url, ".jpg", @"-xs.jpg", RegexOptions.IgnoreCase)}";
            
            ExtractIngredientsAndInstructions(post.Body);
        }

        private void ExtractIngredientsAndInstructions(string body)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(body);

            var nodes = doc.DocumentNode.SelectNodes("ol");
            if (nodes.Count >= 2)
            {
                recipe.Ingredients = nodes[0].ChildNodes.Where(x=>!x.InnerText.IsBlank()).Select(x => x.InnerText).ToList();
                foreach (var n in nodes.Where(n => n != nodes[0]))
                    recipe.Instructions.AddRange(n.ChildNodes.Where(x => !x.InnerText.IsBlank()).Select(x => x.InnerText).ToList());                
            }
        }
    }
}