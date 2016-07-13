using Composr.Core;
using Composr.Lib.Util;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Lib.Indexing
{
    public class IndexWriter : IIndexWriter
    {
        private ILogger logger;
        private Directory indexDirectory;

        public IndexWriter()
        {
            logger = Logging.CreateLogger<IndexWriter>();            
            indexDirectory = FSDirectory.Open(new System.IO.DirectoryInfo(Settings.IndexDirectory));
        }
        
        public void GenerateIndex(IList<Post> posts)
        {
            ComposrAnalyzer analyzer = new ComposrAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var indexWriter = new Lucene.Net.Index.IndexWriter(indexDirectory, analyzer, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var post in posts)
                {
                    Document doc = CreateDocument(post);
                    indexWriter.AddDocument(doc);
                }
                indexWriter.Commit();
                indexWriter.Optimize();
            }
        }

        private Document CreateDocument(Post post)
        {
            Document doc = new Document();
            doc.Add(new Field(IndexFields.BlogID, post.Blog.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(IndexFields.Locale, post.Blog.Locale.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            //doc.Add(new Field(IndexFields.IndexedPostBody, post.Body.StripHTMLTags().StripLineFeedCarriageReturn(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(IndexFields.PostBody, post.Body, Field.Store.YES, Field.Index.NO));

            doc.Add(new Field(IndexFields.PostDatePublished, post.DatePublished.Value.ToString("MMM d, yyyy"), Field.Store.YES, Field.Index.NO));
            doc.Add(new Field(IndexFields.PostDatePublishedTicks, post.DatePublished.Value.ToString("yyyyMMddhhmmss"), Field.Store.NO, Field.Index.NOT_ANALYZED));

            doc.Add(new Field(IndexFields.PostID, post.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            doc.Add(new Field(IndexFields.PostTitle, post.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(IndexFields.PostURN, post.URN, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

            if (post.Attributes.ContainsKey(PostAttributeKeys.MetaDescription) && !string.IsNullOrWhiteSpace(post.Attributes[PostAttributeKeys.MetaDescription]))
                doc.Add(new Field(IndexFields.PostMetaDescription, post.Attributes[PostAttributeKeys.MetaDescription], Field.Store.YES, Field.Index.ANALYZED));

            if (post.Attributes.ContainsKey(PostAttributeKeys.Tags) && !string.IsNullOrWhiteSpace(post.Attributes[PostAttributeKeys.Tags]))
                doc.Add(new Field(IndexFields.Tags, post.Attributes[PostAttributeKeys.Tags], Field.Store.YES, Field.Index.NO));

            string snippet = PrepareSnippet(post.Body);
            doc.Add(new Field(IndexFields.PostSnippet, snippet, Field.Store.YES, Field.Index.NO));
            bool hasImage = false;

            if(post.Images.Count > 0)
            {
                var img = post.Images.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(img.Url))
                {
                    hasImage = true;
                    doc.Add(new Field(IndexFields.ImageUrl, post.Blog.Attributes[BlogAttributeKeys.ImageLocation] + img.Url, Field.Store.YES, Field.Index.NO));
                    if (!string.IsNullOrWhiteSpace(img.Caption)) doc.Add(new Field(IndexFields.ImageCaption, img.Caption, Field.Store.YES, Field.Index.NO));
                }
            }

            doc.Add(new Field(IndexFields.HasImage, (hasImage? "y": "n"), Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));

            return doc;
        }

        private static string PrepareSnippet(string source)
        {
            string snippet = source.Trim()
                                    .GetFirstHtmlParagraph()
                                    .StripHTMLTags()
                                    .StripLineFeedCarriageReturn()
                                    .StripConsecutiveSpaces();

            if (snippet.Length > IndexSettings.MaxSnippetLength)
                return snippet.Substring(0, IndexSettings.MaxSnippetLength);

            return snippet;
        }
    }
}
