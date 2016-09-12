using Composr.Core;
using Microsoft.AspNetCore.Mvc;

namespace Composr.Web.Models
{
    public class TranslateRequestModel
    {
        [FromForm(Name = "Url")]
        public string Url { get; set; }

        [FromForm(Name = "SourceLocale")]
        public Locale SourceLocale { get; set; }

        [FromForm(Name = "TargetLocale")]
        public Locale TargetLocale { get; set; }
    }
}
