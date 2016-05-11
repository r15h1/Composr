using System;
using Microsoft.Extensions.Configuration;

namespace Composr.Util
{
    public static class Configuration
    {
        private static IConfigurationRoot config;

        static Configuration()
        {
            InitializeConfiguration();
        }

        private static void InitializeConfiguration()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("settings.json");
            builder.AddEnvironmentVariables();
            config = builder.Build();
        }

        public static string ConnectionString {
            get
            {
                return config["Data:ConnectionString"];
            }
        }

        public static string IndexDirectory
        {
            get
            {
                return config["Index:Directory"];
            }
        }
    }
}
