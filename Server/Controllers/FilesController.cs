using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BootGen;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace StartVue.Controllers
{
    [ApiController]
    [Route("files")]
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
            Console.WriteLine(id);
            Console.WriteLine(filename);
            if (string.IsNullOrWhiteSpace(filename))
                filename = "index.html";
            Console.WriteLine(filename);
            string key = $"{id}/{filename}";
            Console.WriteLine(key);
            string content;
            if (!memoryCache.TryGetValue(key, out content))
                return NotFound();
            //memoryCache.Remove(key);

            string contentType;
            Console.WriteLine(content.Substring(0, 20).Replace("\n", @"\n"));

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
            Console.WriteLine(contentType);
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
