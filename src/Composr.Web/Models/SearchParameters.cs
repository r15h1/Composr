using Microsoft.AspNetCore.Mvc;

namespace Composr.Web.Models
{
    public class SearchParameters
    {
        /// <summary>
        /// the search term that the user typed
        /// </summary>
        [FromQuery(Name = "q")]
        public string Query { get; set; }

        /// <summary>
        /// the category (one or more, delimited) that would be used to filter results
        /// </summary>
        [FromQuery(Name = "cat")]
        public string Category { get; set; }

        /// <summary>
        /// the current page
        /// </summary>
        [FromQuery(Name = "page")]
        public int? Page { get; set; } = 1;
    }
}
