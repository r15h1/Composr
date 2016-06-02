using Microsoft.Extensions.Logging;

namespace Composr.Lib.Util
{
    public class Logging
    {
        static Logging()
        {
            LoggerFactory = new LoggerFactory().AddConsole().AddDebug();
        }

        private static ILoggerFactory LoggerFactory;

        public static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();
    }
}
