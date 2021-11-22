using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;
using VueStart.Services;
using VueStart.Data;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/generate")]
    public class GenerationController : ControllerBase
    {
        private readonly GenerationService generateService;
        private readonly StatisticsService statisticsService;

        public GenerationController(GenerationService generateService, StatisticsService statisticsService)
        {
            this.generateService = generateService;
            this.statisticsService = statisticsService;
        }

        [HttpPost]
        [Route("{type}/{layout}")]
        public IActionResult Generates([FromBody] JsonElement json, string type, string layout)
        {
            var artifactType = type.ToArtifactType();
            if (artifactType == ArtifactType.None)
                return NotFound();
            statisticsService.OnEvent(Request.HttpContext, json.ToString(), ActionType.Generate, artifactType);
            string artifactId = generateService.GenerateToCache(json, $"Data {ToUpperFirst(type)}", $"{type}-{layout}.sbn");
            statisticsService.OnGenerateEnd();
            return base.Ok(new { Id = artifactId });
        }

        private static string ToUpperFirst(string type)
        {
            return Char.ToUpper(type.First()) + type.Substring(1);
        }
    }
}
