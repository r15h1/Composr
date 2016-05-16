using Composr.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.ViewModels
{
    public class PostSearchViewModel:BaseFrontEndViewModel
    {
        public static PostSearchViewModel FromBaseFrontEndViewModel(BaseFrontEndViewModel model)
        {
            return new PostSearchViewModel()
            {
                BlogUrl = model.BlogUrl,
                Copyright = model.Copyright,
                LogoUrl = model.LogoUrl,
                Tagline = model.Tagline
            };
        }

        public IList<SearchResult> SearchResults { get; set; }
    }
}
