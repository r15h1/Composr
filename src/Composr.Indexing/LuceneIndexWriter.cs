using Composr.Core;
using Composr.Util;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Composr.Indexing
{
    public class LuceneIndexWriter : IIndexWriter
    {
        private ILogger logger;
        private Directory indexDirectory;

        public LuceneIndexWriter()
        {
            logger = Logging.CreateLogger<LuceneIndexWriter>();
            indexDirectory = FSDirectory.Open(new System.IO.DirectoryInfo(Configuration.IndexDirectory));
        }

        //public void AddOrUpdate(Post post)
        //{
        //    logger.LogInformation($"adding {post.Title} to index");
        //    Document doc = CreateDocument(post);
        //    UpdateIndex(doc);
        //}

        //private void UpdateIndex(Document doc)
        //{
        //    StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        //    using (var indexWriter = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
        //    {
        //        indexWriter.AddDocument(doc);                
        //        indexWriter.Commit();
        //        indexWriter.Optimize();
        //    }
        //}

        //public bool Delete(Post post)
        //{
        //    throw new NotImplementedException();
        //}

        public void GenerateIndex(IList<Post> posts)
        {
            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (var indexWriter = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
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
            doc.Add(new Field(IndexFields.BlogID, post.Blog.Id.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
            doc.Add(new Field(IndexFields.Locale, post.Blog.Locale.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS));
            doc.Add(new Field(IndexFields.PostBody, post.Body, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(IndexFields.PostDatePublished, post.DatePublished.Value.ToString("dd/MM/yyyy"), Field.Store.YES, Field.Index.NO));
            doc.Add(new Field(IndexFields.PostID, post.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            doc.Add(new Field(IndexFields.PostTitle, post.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(IndexFields.PostURN, post.URN, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

            if (post.Attributes.ContainsKey(PostAttributeKeys.MetaDescription) && !string.IsNullOrWhiteSpace(post.Attributes[PostAttributeKeys.MetaDescription]))
                doc.Add(new Field(IndexFields.PostMetaDescription, post.Attributes[PostAttributeKeys.MetaDescription], Field.Store.YES, Field.Index.NO));

            return doc;
        }
    }
}
