using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core
{
    public class Category
    {
        public Category(Blog blog)
        {
            if (blog == null) throw new ArgumentNullException();
            this.Blog = blog;
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string URN { get; set; }
        public IEnumerable<Category> Translations { get; set; }
        public Locale Locale
        {
            get{
                return Blog.Locale.Value;
            }
        }

        public Blog Blog { get; private set; }
    }
}
