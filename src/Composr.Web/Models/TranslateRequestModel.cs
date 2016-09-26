using Composr.Core;
using Microsoft.AspNetCore.Mvc;

namespace Composr.Web.Models
{
    public class TranslateRequestModel
    {
        [FromQuery(Name = "Url")]
        public string Url { get; set; }

        [FromQuery(Name = "sl")]
        public Locale SourceLocale { get; set; }

        [FromQuery(Name = "tl")]
        public Locale TargetLocale { get; set; }
    }
}
