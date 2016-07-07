using Microsoft.Extensions.Configuration;

namespace Composr.Lib.Util
{
    public static class Settings
    {
        public static IConfigurationRoot Config { get; set; }        

        public static string ConnectionString
        {
            get
            {
                return Config["Data:ConnectionString"];
            }
        }

        public static string IndexDirectory
        {
            get
            {
                return Config["Index:Directory"];
            }
        }

        public static string ImageSourceDirectory
        {
            get
            {
                return Config["ImageLocation:SourceDirectory"];
            }
        }

        public static string ImageDestinationDirectory
        {
            get
            {
                return Config["ImageLocation:DestinationDirectory"];
            }
        }
    }
}
