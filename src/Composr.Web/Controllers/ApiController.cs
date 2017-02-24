using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Composr.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {
        private IHostingEnvironment environment;
        public ApiController(IHostingEnvironment environment, Blog blog)
        {
            this.environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IList<object> files)
        {
            var uploads = Path.Combine(environment.WebRootPath, "temp\\uploads");
            foreach (var file in Request.Form.Files)
            {
                if (file.Length > 0)
                {
                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            return new JsonResult("file uploaded successfully");
        }

        [HttpGet("upload")]
        public IActionResult Upload(ICollection<IFormFile> files)
        {
            long size = 0;
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');

                filename = "C:\\" + $@"\{filename}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            return new JsonResult("hello");
        }
    }
}
