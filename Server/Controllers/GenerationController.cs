using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;
using VueStart.Services;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("")]
    public class GenerationController : ControllerBase
    {
        private readonly GenerateService generateService;
        private readonly StatisticsService statisticsService;

        public GenerationController(GenerateService generateService, StatisticsService statisticsService)
        {
            this.generateService = generateService;
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
            string artifactId = generateService.Generate(json, $"Data {ToUpperFirst(type)}", $"{type}-{layout}.sbn");
            statisticsService.OnGenerateEnd();
            return base.Ok(new { Id = artifactId });
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
            statisticsService.OnDownloadEnd();
            return File(memoryStream, "application/zip", $"{type}.zip");
        }

        private MemoryStream CreateZipStream(JsonElement json, string title, string templateFileName)
        {
            var memoryStream = new MemoryStream();
            generateService.Generate(json, title, templateFileName, out string appjs, out string indexhtml);
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
    }
}
