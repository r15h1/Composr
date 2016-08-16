using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        private static int searchPageSize;
        public static int DefaultSearchPageSize
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Config["Search:DefaultPageSize"]) 
                    && int.TryParse(Config["Search:DefaultPageSize"], out searchPageSize)
                    )
                    return searchPageSize;

                return 16;
            }
        }

        private static int maxResultsCount;
        public static int MaxSearchResultsCount
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Config["Search:MaxResultsCount"])
                    && int.TryParse(Config["Search:MaxResultsCount"], out maxResultsCount)
                    )
                    return maxResultsCount;

                return 1000;
            }
        }
    }    
}