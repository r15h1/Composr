using Microsoft.Extensions.Configuration;

namespace Composr.Lib.Util
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

        public static string ImageSourceDirectory
        {
            get
            {
                return config["ImageLocation:SourceDirectory"];
            }
        }

        public static string ImageDestinationDirectory
        {
            get
            {
                return config["ImageLocation:DestinationDirectory"];
            }
        }
    }
}
