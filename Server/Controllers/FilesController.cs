using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BootGen;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        public FilesController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("{id}/{filename}")]
        public IActionResult ServeFile([FromRoute] string id, [FromRoute] string filename) {
            if (string.IsNullOrWhiteSpace(filename))
                filename = "index.html";
            string key = $"{id}/{filename}";
            string content;
            if (!memoryCache.TryGetValue(key, out content))
                return NotFound();

            string contentType;

            switch (filename.Split('.').LastOrDefault())
            {
                case "js":
                    contentType = "application/javascript";
                break;
                case "html":
                    contentType = "text/html";
                break;
                default:
                    contentType = "text/plain";
                break;
            }
            var cd = new System.Net.Mime.ContentDisposition
            {
                    FileName = filename,
                    Inline = true
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return File(Encoding.UTF8.GetBytes(content), contentType);
        }

        private static VirtualDisk Load(string path)
        {
            var templates = new VirtualDisk();
            foreach (var file in Directory.EnumerateFiles(path))
            {
                templates.Files.Add(new VirtualFile
                {
                    Name = Path.GetFileName(file),
                    Path = "",
                    Content = System.IO.File.ReadAllText(file)
                });
            }

            return templates;
        }
    }
}
