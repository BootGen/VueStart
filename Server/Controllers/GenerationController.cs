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
    [Route("generate")]
    public class GenerationController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        public GenerationController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        [HttpPost]
        public IActionResult Generate([FromBody] JsonElement json)
        {
            var dataModel = new DataModel();
            var jObject = JObject.Parse(json.ToString(), new JsonLoadSettings { CommentHandling = CommentHandling.Ignore, DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            dataModel.Load(jObject);
            var collection = new ResourceCollection(dataModel);
            var seedStore = new SeedDataStore(collection);
            seedStore.Load(jObject);

            var id = Guid.NewGuid().ToString();
            var generator = new TypeScriptGenerator(null);
            generator.Templates = Load("../templates");
            string appjs = generator.Render("app.sbn", new Dictionary<string, object> {
                {"classes", dataModel.CommonClasses}
            });
            string indexhtml = generator.Render("index.sbn", new Dictionary<string, object> {
                {"base_url", $"http://localhost:8080/files/{id}/"}
            });


            memoryCache.Set($"{id}/app.js", Minify(appjs), TimeSpan.FromMinutes(1));
            memoryCache.Set($"{id}/index.html", Minify(indexhtml), TimeSpan.FromMinutes(1));

            return Ok(new { Id = id });
        }

        private string Minify(string value) {
            value = value.Replace("\n", " ");
            value = value.Replace("\r", " ");
            value = value.Replace("\t", " ");
            int length;
            do {
                length = value.Length;
                value = value.Replace("  ", " ");
            } while(value.Length != length);

            return value;
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
