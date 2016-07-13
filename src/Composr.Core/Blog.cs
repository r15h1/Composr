using System;
using System.Collections.Generic;

namespace Composr.Core
{
    public class Blog: IComposrEntity
    {
        public Blog()
        {
            Init();
        }
        
        public Blog(int blogid)
        {
            Init();
            this.Id = blogid;
        }

        private void Init()
        {
            Attributes = new Dictionary<string, string>();
            Locale = Composr.Core.Locale.EN;
        }

        int? blogid;
        public int? Id {
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

        public string Theme
        {
            get
            {
                if (Attributes.ContainsKey(BlogAttributeKeys.Theme))
                    return Attributes[BlogAttributeKeys.Theme];
                return null;
            }
        }

        public Dictionary<string, string> Attributes { get; set; }
    }
}