using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;
using VueStart.Services;
using VueStart.Data;
using BootGen.Core;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/generate")]
    public class GenerationController : ControllerBase
    {
        private readonly GenerationService generationService;
        private readonly StatisticsService statisticsService;

        public GenerationController(GenerationService generateService, StatisticsService statisticsService)
        {
            this.generationService = generateService;
            this.statisticsService = statisticsService;
        }

        [HttpPost]
        public IActionResult Generate([FromBody] GenerateRequest request)
        {
            var layout = request.Settings.Layout;
            var type = request.Settings.Type;
            var color = request.Settings.Color;
            var json = request.Data;
            var artifactType = layout.ToArtifactType();
            if (artifactType == ArtifactType.None)
                return NotFound();
            var cssType = type.ToCssType();
            if (cssType == CssType.None)
                return NotFound();
            if (json.ValueKind != JsonValueKind.Object) {
                statisticsService.OnEvent(Request.HttpContext, json, ActionType.Generate, artifactType, cssType, true);
                return BadRequest(new { error = "The root element must be an object!", fixable = true });
            }
            foreach (var property in json.EnumerateObject()) {
                if (property.Value.ValueKind != JsonValueKind.Object && property.Value.ValueKind != JsonValueKind.Array) {
                    statisticsService.OnEvent(Request.HttpContext, json, ActionType.Generate, artifactType, cssType, true);
                    return BadRequest(new { error = "Properties of the root element must be an objects or arrays!", fixable = false });
                }
            }
            try {
                statisticsService.OnEvent(Request.HttpContext, json, ActionType.Generate, artifactType, cssType);
                string artifactId = generationService.GenerateToCache(json, $"Data {ToUpperFirst(layout)}", $"{type}-{layout}.sbn", type, color, out var warnings);
                statisticsService.OnGenerateEnd();
                return Ok(new { Id = artifactId, Warnings = warnings });
            } catch (FormatException e) {
                return BadRequest(new { error = e.Message, fixable = false });
            } catch (NamingException e) {
                return BadRequest(new { error = e.Message, fixable = true });
            }
        }

        [HttpPost]
        [Route("fix")]
        public IActionResult Fix([FromBody] JsonElement json)
        {
            return Ok(generationService.Fix(json));
        }
        private static string ToUpperFirst(string type)
        {
            return Char.ToUpper(type.First()) + type.Substring(1);
        }
    }
}
