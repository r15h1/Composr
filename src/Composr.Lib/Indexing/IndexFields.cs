namespace Composr.Lib.Indexing
{
    internal class IndexFields
    {
        public static readonly string BlogID = "blgid";
        public static readonly string Locale = "loc";
        public static readonly string PostID = "pid";
        public static readonly string PostTitle = "titl";
        public static readonly string IndexedPostBody = "indexedbody";
        public static readonly string PostBody = "body";
        public static readonly string PostSnippet = "snipet";
        public static readonly string PostURN = "urn";
        public static readonly string PostDatePublished = "pubdate";
        public static readonly string PostDatePublishedTicks = "pubticks";
        public static readonly string PostMetaDescription = "metadsc";
        public static readonly string MainIngredient = "ingrd";
        public static readonly string Yield ="yield";
        public static readonly string Tags = "tags";
        public static readonly string ImageUrl = "imgurl";
        public static readonly string ImageCaption = "imgcap";
        public static readonly string HasImage = "hasimg";
    }  
    
    internal class IndexSettings
    {
        public const int MaxSnippetLength = 500;
    }  
}