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

            const string targetFolder = "../DefaultSite/src";
            var disk = new Disk(targetFolder);
            
            foreach( var file in Load("../BootgenPlugin/files").Files) {
                var dir = Path.Combine("../DefaultSite", file.Path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                System.IO.File.WriteAllText(Path.Combine(dir, file.Name), file.Content);
            }
            var project = new ClientProject
            {
                Config = new ClientConfig {
                    ComponentsFolder = "components",
                    ViewsFolder = "components",
                    ComponentExtension = "vue",
                    Extension = "js"
                },
                Disk = disk,
                ResourceCollection = collection,
                SeedStore = seedStore,
                Templates = Load("../BootgenPlugin/templates")
            };
            project.GenerateFiles("Dummy", "http://localhost:5000");
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
