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

namespace StartVue.Controllers
{
    [ApiController]
    [Route("generate")]
    public class GenerationController : ControllerBase
    {
        [HttpPost]
        public IActionResult Generate([FromBody] JsonElement json)
        {
            var dataModel = new DataModel();
            var jObject = JObject.Parse(json.ToString(), new JsonLoadSettings { CommentHandling = CommentHandling.Ignore, DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            dataModel.Load(jObject);
            var collection = new ResourceCollection(dataModel);
            var seedStore = new SeedDataStore(collection);
            seedStore.Load(jObject);

            const string targetFolder = "/var/www/sites/default";
            var disk = new Disk(targetFolder);
            var generator = new TypeScriptGenerator(disk);
            generator.Templates = Load("../templates");
            generator.Render("", "app.js", "app.sbn", new Dictionary<string, object> {
                {"classes", DataModel.CommonClasses}
            });
            return Ok();
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
