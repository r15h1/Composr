using Composr.Util;
using Microsoft.Extensions.Logging;

namespace Composr.Indexing.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILogger logger = Logging.CreateLogger<Program>();
            logger.LogInformation("starting indexing program");
            new LuceneClient(new LuceneIndexWriter()).GenerateIndex();
            logger.LogInformation("indexing completed");
        }
    }
}