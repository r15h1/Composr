using System;

namespace Composr.Core
{
    public class Blog
    {
        public Blog()
        {
            Init();
        }
        
        public Blog(int blogid)
        {
            Init();
            this.BlogID = blogid;
        }

        private void Init()
        {
            Locale = Composr.Core.Locale.EN;
        }

        int? blogid;
        public int? BlogID {
            get { return blogid; }
            set
            {
                if (value.HasValue && value.Value <= 0) throw new ArgumentException("value must be postive greater than 0");
                blogid = value;
            }
                 
        }

        public Locale? Locale { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
        
        public string Url { get; set; }
        
    }
}
