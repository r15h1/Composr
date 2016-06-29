using Composr.Lib.Indexing;
using Composr.Lib.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Composr.Indexing.Client
{
    public class Program
    {
        static ILogger logger = Logging.CreateLogger<Program>();
        static Dictionary<string, Delegate> commands = new Dictionary<string, Delegate>()
        {
            {"index", new Action(()=> GenerateIndex()) },
            {"image", new Action(()=> new ImageClient().ProcessImages()) }
        };

        public static void Main(string[] args)
        {
            if (args.Length == 1 && commands.ContainsKey(args[0].ToLowerInvariant().Trim()))
                ExecuteCommand(args);            
            else
                logger.LogInformation("Usage: dnx Composr.Indexing.Client [arg] where [arg] = \"index\" OR \"image\"");
        }

        private static void ExecuteCommand(string[] args)
        {
            try
            {
                commands[args[0].ToLowerInvariant().Trim()].DynamicInvoke();
            }
            catch (Exception ex)
            {
                logger.LogCritical($"error: {ex.Message + Environment.NewLine + ex.StackTrace}");
            }
        }

        private static void GenerateIndex()
        {
            logger.LogInformation("starting indexing program");

            logger.LogInformation($"deleting current index in {Configuration.IndexDirectory}");
            ClearIndexDirectory();

            new LuceneClient(new LuceneIndexWriter()).GenerateIndex();
            logger.LogInformation("indexing completed");
        }

        private static void ClearIndexDirectory()
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(Configuration.IndexDirectory))
                System.IO.File.Delete(file);            
        }
    }
}