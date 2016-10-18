namespace Composr.Lib.Indexing
{
    internal class IndexFields
    {
        public static readonly string BlogID = "blgid";
        public static readonly string Locale = "loc";
        public static readonly string PostID = "pid";
        public static readonly string Title = "ptitl";
        public static readonly string IndexedPostBody = "pidxbdy";
        public static readonly string PostBody = "pbdy";
        public static readonly string PostSnippet = "psnip";
        public static readonly string URN = "purn";
        public static readonly string PostDatePublished = "pubdat";
        public static readonly string PostDatePublishedTicks = "pubtic";
        public static readonly string PostMetaDescription = "pmetadsc";
        public static readonly string MainIngredient = "pingrd";
        public static readonly string Yield ="pyld";
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