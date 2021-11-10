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
using System.IO.Compression;
using VueStart.Services;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("")]
    public class GenerationController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly StatisticsService statisticsService;

        public GenerationController(IMemoryCache memoryCache, StatisticsService statisticsService)
        {
            this.memoryCache = memoryCache;
            this.statisticsService = statisticsService;
        }

        [HttpPost]
        [Route("generate/editor/{layout}")]
        public IActionResult GenerateEditor([FromBody] JsonElement json, string layout)
        {
            statisticsService.onEvent(Request.HttpContext, json.ToString(), ActionType.Generate, ArtifactType.Editor);
            return Ok(new { Id = Generate(json, "Data Editor", $"editor-{layout}.sbn") });
        }
        
        [HttpPost]
        [Route("generate/view/{layout}")]
        public IActionResult GenerateView([FromBody] JsonElement json, string layout)
        {
            statisticsService.onEvent(Request.HttpContext, json.ToString(), ActionType.Generate, ArtifactType.View);
            return Ok(new { Id = Generate(json, "Data View", $"view-{layout}.sbn") });
        }

        [HttpPost]
        [Route("generate/form/{layout}")]
        public IActionResult GenerateForm([FromBody] JsonElement json, string layout)
        {
            statisticsService.onEvent(Request.HttpContext, json.ToString(), ActionType.Generate, ArtifactType.Form);
            return Ok(new { Id = Generate(json, "Data Form", $"form-{layout}.sbn") });
        }

        [HttpPost]
        [Route("download/editor/{layout}")]
        public IActionResult DownloadEditor([FromBody] JsonElement json, string layout)
        {
            statisticsService.onEvent(Request.HttpContext, json.ToString(), ActionType.Download, ArtifactType.Editor);
            var memoryStream = CreateZipStream(json, "Data Editor", $"editor-{layout}.sbn");
            return File(memoryStream, "application/zip", "editor.zip");
        }

        [HttpPost]
        [Route("download/view/{layout}")]
        public IActionResult DownloadView([FromBody] JsonElement json, string layout)
        {
            statisticsService.onEvent(Request.HttpContext, json.ToString(), ActionType.Download, ArtifactType.View);
            var memoryStream = CreateZipStream(json, "Data View", $"view-{layout}.sbn");
            return File(memoryStream, "application/zip", "view.zip");
        }

        [HttpPost]
        [Route("download/form/{layout}")]
        public IActionResult DownloadForm([FromBody] JsonElement json, string layout)
        {
            statisticsService.onEvent(Request.HttpContext, json.ToString(), ActionType.Download, ArtifactType.Form);
            var memoryStream = CreateZipStream(json, "Data Form", $"form-{layout}.sbn");
            return File(memoryStream, "application/zip", "form.zip");
        }

        private MemoryStream CreateZipStream(JsonElement json, string title, string templateFileName)
        {
            var memoryStream = new MemoryStream();
            Generate(json, title, templateFileName, out string appjs, out string indexhtml);
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                AddEntry(archive, appjs, "app.js");
                AddEntry(archive, indexhtml, "index.html");
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static void AddEntry(ZipArchive archive, string content, string fileName)
        {
            var entry = archive.CreateEntry(fileName);

            using (var entryStream = entry.Open())
            using (var streamWriter = new StreamWriter(entryStream))
            {
                streamWriter.Write(content);
            }
        }

        private string Generate(JsonElement json, string title, string templateFileName, out string appjs, out string indexhtml) {
            var dataModel = new DataModel();
            var jObject = JObject.Parse(json.ToString(), new JsonLoadSettings { CommentHandling = CommentHandling.Ignore, DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            dataModel.LoadRootObject("App", jObject);
            var collection = new ResourceCollection(dataModel);
            var seedStore = new SeedDataStore(collection);
            seedStore.Load(jObject);

            var id = Guid.NewGuid().ToString();
            var generator = new TypeScriptGenerator(null);
            generator.Templates = Load("templates");
            appjs = generator.Render(templateFileName, new Dictionary<string, object> {
                {"classes", dataModel.CommonClasses}
            });
            indexhtml = generator.Render("index.sbn", new Dictionary<string, object> {
                {"base_url", $"/files/{id}/"},
                {"title", $"{title}"}
            });
            return id;
        }

        private string Generate(JsonElement json, string title, string templateFileName) {
            string id = Generate(json, title, templateFileName, out string appjs, out string indexhtml);
            memoryCache.Set($"{id}/app.js", Minify(appjs), TimeSpan.FromMinutes(1));
            memoryCache.Set($"{id}/index.html", Minify(indexhtml), TimeSpan.FromMinutes(1));
            return id;
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
