using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.ViewModels
{
    public class PostViewModel
    {
        public int? Id { get; set; }

        public string Title { get; set; }
        
        public string Body { get; set; }

        public int? BlogId { get; set; }
    }
}