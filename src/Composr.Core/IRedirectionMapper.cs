using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core
{
    public interface IRedirectionMapper
    {
        bool CanResolve(string url);

        string MapToRedirectUrl(string originalUrl);
    }
}
