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
using System.Threading.Channels;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/generate")]
    public class GenerationController : ControllerBase
    {
        private readonly GenerationService generationService;
        private readonly Channel<EventData> eventChannel;

        public GenerationController(GenerationService generateService, Channel<EventData> eventChannel)
        {
            this.generationService = generateService;
            this.eventChannel = eventChannel;
        }

        [HttpPost]
        public IActionResult Generate([FromBody] GenerateRequest request)
        {
            var eventData = new EventData(Request.HttpContext, request, ActionType.Generate);
            if (request.Data.ValueKind != JsonValueKind.Object) {
                eventData.Error = true;
                eventChannel.Writer.WriteAsync(eventData);
                return BadRequest(new { error = "The root element must be an object!", fixable = true });
            }
            foreach (var property in request.Data.EnumerateObject()) {
                if (property.Value.ValueKind != JsonValueKind.Object && property.Value.ValueKind != JsonValueKind.Array) {
                    eventData.Error = true;
                    eventChannel.Writer.WriteAsync(eventData);
                    return BadRequest(new { error = "Properties of the root element must be an objects or arrays!", fixable = false });
                }
            }
            try {
                eventChannel.Writer.WriteAsync(eventData);
                var result = generationService.GenerateToCache(request, "DataTable");
                return Ok(result);
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
