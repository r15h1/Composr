using System;

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

        private string theme;
        public string Theme
        {
            get
            {
                return string.IsNullOrWhiteSpace(theme) ? "Default" : theme;
            }
            set
            {
                theme = value;
            }
        }

        private string logo;
        public string Logo
        {
            get
            {
                return string.IsNullOrWhiteSpace(logo) ? "~/img/Composr.png" : "~/img/Cocozil.png";
            }
            set
            {
                logo = value;
            }
        }
    }
}