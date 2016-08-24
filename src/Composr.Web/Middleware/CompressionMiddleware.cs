using Microsoft.AspNetCore.Http;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Composr.Web.Middleware
{
    public class CompressionMiddleware
    {
        private readonly RequestDelegate _next;

        public CompressionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsGZipSupported(context))
            {
                string acceptEncoding = context.Request.Headers["Accept-Encoding"];

                var buffer = new MemoryStream();
                var stream = context.Response.Body;
                context.Response.Body = buffer;
                await _next(context);

                if (acceptEncoding.Contains("gzip"))
                {
                    var gstream = new GZipStream(stream, CompressionLevel.Optimal);
                    context.Response.Headers.Add("Content-Encoding", new[] { "gzip" });
                    buffer.Seek(0, SeekOrigin.Begin);
                    await buffer.CopyToAsync(gstream);
                    gstream.Dispose();
                }
                else
                {
                    var gstream = new DeflateStream(stream, CompressionLevel.Optimal);
                    context.Response.Headers.Add("Content-Encoding", new[] { "deflate" });
                    buffer.Seek(0, SeekOrigin.Begin);
                    await buffer.CopyToAsync(gstream);
                    gstream.Dispose();
                }
            }
            else
            {
                await _next(context);
            }
        }

        public bool IsGZipSupported(HttpContext context)
        {
            string acceptEncoding = context.Request.Headers["Accept-Encoding"];
            return !string.IsNullOrEmpty(acceptEncoding) &&
                   (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate"));
        }
    }
}