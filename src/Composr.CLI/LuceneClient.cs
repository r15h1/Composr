using System.Collections.Generic;
using Composr.Core;
using Microsoft.Extensions.Logging;
using Composr.Lib.Util;
using Composr.Lib.Indexing;

namespace Composr.CLI
{
    public class LuceneClient
    {
        private ILogger logger = Logging.CreateLogger<LuceneClient>();

        public LuceneClient()
        {
        }

        internal void GenerateIndex()
        {
            logger.LogInformation("getting list of blogs");
            IBlogRepository repo = new Composr.Repository.Sql.BlogRepository();
            IList<Blog> blogs = repo.Get(null);
            var generator = new IndexGenerator(new IndexWriter());
            foreach (var blog in blogs) generator.BuildIndex(blog);
        }        
    }
}
