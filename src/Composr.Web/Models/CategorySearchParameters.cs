using Composr.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.Models
{
    public class CategorySearchParameters
    {
        [FromRoute(Name ="locale")]
        public Locale Locale { get; set; }

        [FromRoute(Name = "region")]
        public string Region { get; set; }

        [FromRoute(Name = "category")]
        public string Category { get; set; }
    }
}
