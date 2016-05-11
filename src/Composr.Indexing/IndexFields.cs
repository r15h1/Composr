using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Indexing
{
    internal class IndexFields
    {
        public const string BlogID = "blogid";
        public const string Locale = "loc";
        public const string PostID = "pid";
        public const string PostTitle = "title";
        public const string PostBody = "body";
        public const string PostSnippet = "snippet";
        public const string PostURN = "urn";
        public const string PostDatePublished = "pubdate";
        public const string PostMetaDescription = "metadesc";
    }  
    
    internal class IndexSettings
    {
        public const int MaxSnippetLength = 300;
    }  
}
