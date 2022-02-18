using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.IO.Compression;
using VueStart.Services;
using VueStart.Data;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController : ControllerBase
    {
        private readonly GenerationService generateService;
        private readonly StatisticsService statisticsService;

        public DownloadController(GenerationService generateService, StatisticsService statisticsService)
        {
            this.generateService = generateService;
            this.statisticsService = statisticsService;
        }

        private static string ToUpperFirst(string type)
        {
            return Char.ToUpper(type.First()) + type.Substring(1);
        }

        [HttpPost]
        [Route("{type}/{layout}/{color}")]
        public IActionResult DownloadEditor([FromBody] JsonElement json, string type, string layout, string color)
        {
            try {
                var artifactType = layout.ToArtifactType();
                if (artifactType == ArtifactType.None)
                    return NotFound();
                statisticsService.OnEvent(Request.HttpContext, json.ToString(), ActionType.Download, artifactType);
                var memoryStream = CreateZipStream(json, $"Data {ToUpperFirst(layout)}", $"{type}-{layout}.sbn", color);
                statisticsService.OnDownloadEnd();
                return File(memoryStream, "application/zip", $"{layout}.zip");
            } catch (FormatException e) {
                return BadRequest(new { error = e.Message });
            }
        }

        private MemoryStream CreateZipStream(JsonElement json, string title, string templateFileName, string color)
        {
            var memoryStream = new MemoryStream();
            generateService.Generate(json, title, templateFileName, color, true, out string appjs, out string indexhtml);
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
