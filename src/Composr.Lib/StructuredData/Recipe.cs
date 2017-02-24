using Composr.Core;
using Composr.Lib.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Composr.Lib.StructuredData
{
    /// <summary>
    /// based on 
    /// https://developers.google.com/search/docs/data-types/recipes and 
    /// http://schema.org/Recipe
    /// </summary>
    public class Recipe: IStructuredData
    {
        public Recipe()
        {
            Ingredients = new List<string>();
            Instructions = new List<string>();
        }

        public string Name { get; set; }
        public string Description { get; set;}
        public string Image { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
        public string Author { get; set; }
        public string Yield { get; set; }
        public string DatePublished { get; set; }
        public string Category { get; set; }

        public string ToJsonLD()
        {
            if (Image.IsBlank()) return null;

            JObject json = new JObject();
            json["@context"] = "http://schema.org/";
            json["@type"] = "Recipe";
            json["name"] = Name;
            json["image"] = Image;
            json["datePublished"] = DatePublished;
            json["recipeIngredient"] = new JArray(Ingredients);
            json["recipeInstructions"] = new JArray(Instructions);
            json["description"] = Description;
            if (!Author.IsBlank()) {
                var author = new JObject();
                author["@type"] = "Organization";
                author["name"] = Author;
                json["author"] = author;
            }
            return json.ToString();
        }
    }
}
