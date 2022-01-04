using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;
using VueStart.Services;
using VueStart.Data;
using BootGen;

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
        [Route("{type}/{layout}")]
        public IActionResult Generates([FromBody] JsonElement json, string type, string layout)
        {
            try {
                var artifactType = type.ToArtifactType();
                if (artifactType == ArtifactType.None)
                    return NotFound();
                statisticsService.OnEvent(Request.HttpContext, json.ToString(), ActionType.Generate, artifactType);
                string artifactId = generationService.GenerateToCache(json, $"Data {ToUpperFirst(type)}", $"{type}-{layout}.sbn");
                statisticsService.OnGenerateEnd();
                return Ok(new { Id = artifactId });
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
