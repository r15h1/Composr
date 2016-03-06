using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.ViewModels
{
    public class PostViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage ="Title is missing")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Body is missing")]
        public string Body { get; set; }

        public int? BlogId { get; set; }
    }
}