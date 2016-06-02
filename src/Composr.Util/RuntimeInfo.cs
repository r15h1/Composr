using Composr.Core;
using System;
using System.Threading;

namespace Composr.Lib.Util
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class RuntimeInfo
    {
        /// <summary>
        /// utility function to get the current executing culture
        /// </summary>
        /// <returns></returns>
        public static Locale ExecutingLocale
        {
            get
            {
                Locale locale = Locale.EN;
                Enum.TryParse(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper(), out locale);
                return locale;
            }
        }
    }
}
