using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.IO.Compression;
using VueStart.Services;
using VueStart.Data;
using System.Threading.Channels;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/download")]
    public class DownloadController : ControllerBase
    {
        private readonly GenerationService generateService;
        private readonly Channel<EventData> eventChannel;

        public DownloadController(GenerationService generateService, Channel<EventData> eventChannel)
        {
            this.generateService = generateService;
            this.eventChannel = eventChannel;
        }

        private static string ToUpperFirst(string type)
        {
            return Char.ToUpper(type.First()) + type.Substring(1);
        }

        [HttpPost]
        public IActionResult DownloadEditor([FromBody] GenerateRequest request)
        {
            var layout = request.Settings.IsReadonly;
            var json = request.Data;
            try {
                var frontend = request.Settings.Frontend.ToFrontendType();
                if (frontend == Frontend.None)
                    return NotFound();
                eventChannel.Writer.WriteAsync(new EventData {
                    Context = Request.HttpContext,
                    Request = request,
                    ActionType = ActionType.Download
                });
                var memoryStream = CreateZipStream(request, "DataTable");
                return File(memoryStream, "application/zip", $"{layout}.zip");
            } catch (FormatException e) {
                return BadRequest(new { error = e.Message });
            }
        }

        private MemoryStream CreateZipStream(GenerateRequest request, string title)
        {
            var memoryStream = new MemoryStream();
            generateService.Generate(request, title, "", true, out string appjs, out string indexhtml);
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
