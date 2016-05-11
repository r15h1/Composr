using Composr.Util;
using Microsoft.Extensions.Logging;
using System;

namespace Composr.Indexing.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILogger logger = Logging.CreateLogger<Program>();
            try {               
                logger.LogInformation("starting indexing program");

                logger.LogInformation($"deleting current index in {Configuration.IndexDirectory}");
                ClearIndexDirectory();

                new LuceneClient(new LuceneIndexWriter()).GenerateIndex();
                logger.LogInformation("indexing completed");
            }
            catch(Exception ex)
            {
                logger.LogCritical($"error: {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        private static void ClearIndexDirectory()
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Configuration.IndexDirectory))
                System.IO.File.Delete(file);            
        }
    }
}