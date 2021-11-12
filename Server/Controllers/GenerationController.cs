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
using System.Diagnostics;

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

        private ArtifactType GetArtifactType(string type)
        {
            switch (type)
            {
                case "editor":
                    return ArtifactType.Editor;
                case "form":
                    return ArtifactType.Form;
                case "view":
                    return ArtifactType.View;
                default:
                    return ArtifactType.None;
            }
        }

        [HttpPost]
        [Route("generate/{type}/{layout}")]
        public IActionResult Generates([FromBody] JsonElement json, string type, string layout)
        {
            var artifactType = GetArtifactType(type);
            if (artifactType == ArtifactType.None)
                return NotFound();
            statisticsService.OnEvent(Request.HttpContext, json.ToString(), ActionType.Generate, artifactType);
            return Ok(new { Id = Generate(json, $"Data {ToUpperFirst(type)}", $"{type}-{layout}.sbn") });
        }

        private static string ToUpperFirst(string type)
        {
            return Char.ToUpper(type.First()) + type.Substring(1);
        }

        [HttpPost]
        [Route("download/{type}/{layout}")]
        public IActionResult DownloadEditor([FromBody] JsonElement json, string type, string layout)
        {
            var artifactType = GetArtifactType(type);
            if (artifactType == ArtifactType.None)
                return NotFound();
            statisticsService.OnEvent(Request.HttpContext, json.ToString(), ActionType.Download, artifactType);
            var memoryStream = CreateZipStream(json, $"Data {ToUpperFirst(type)}", $"{type}-{layout}.sbn");
            return File(memoryStream, "application/zip", $"{type}.zip");
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
            var sw = new Stopwatch();
            sw.Start();
            string id = Generate(json, title, templateFileName, out string appjs, out string indexhtml);
            memoryCache.Set($"{id}/app.js", Minify(appjs), TimeSpan.FromMinutes(1));
            memoryCache.Set($"{id}/index.html", Minify(indexhtml), TimeSpan.FromMinutes(1));
            sw.Stop();
            Console.WriteLine($"Generation time: {sw.ElapsedMilliseconds}");
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
