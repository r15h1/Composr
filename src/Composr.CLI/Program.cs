using Composr.Lib.Indexing;
using Composr.Lib.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.Framework.Runtime;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;

namespace Composr.CLI
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
            InitializeConfiguration();

            if (args.Length == 1 && commands.ContainsKey(args[0].ToLowerInvariant().Trim()))
                ExecuteCommand(args);
            else
                logger.LogInformation("Usage: dnx Composr.Indexing.Client [arg] where [arg] = \"index\" OR \"image\"");
        }

        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder()
                             .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                             .AddJsonFile("settings.json", optional: false, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            Settings.Config = builder.Build();
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
            new LuceneClient().GenerateIndex();
            logger.LogInformation("indexing completed");
        }

        
    }
}