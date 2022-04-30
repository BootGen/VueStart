using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VueStart.Services;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VueStart.Authorization;

namespace VueStart.Controllers;

[Authorize]
[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMemoryCache memoryCache;
    private readonly GenerationService generationService;
    private readonly GenerationService generateService;
    private readonly StatisticsService statisticsService;
    private readonly UserService userService;

    public AdminController(ApplicationDbContext dbContext, IMemoryCache memoryCache, GenerationService generationService, GenerationService generateService, StatisticsService statisticsService, UserService userService)
    {
        this.dbContext = dbContext;
        this.memoryCache = memoryCache;
        this.generationService = generationService;
        this.generateService = generateService;
        this.statisticsService = statisticsService;
        this.userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody]AuthenticateModel model)
    {
        var user = userService.Authenticate(model.Username, model.Password);

        if (user == null)
            return BadRequest(new { message = "Username or password is incorrect" });

        return Ok(user);
    }


    [HttpGet]
    public IActionResult GetDashboard()
    {
        return GetFile("index.html");
    }

    [HttpGet]
    [Route("{fileName}")]
    public IActionResult GetFile(string fileName)
    {
        List<Visitor> visitors = dbContext.Visitors.Include(visitor => visitor.Visits).ToList();
        visitors.AddRange(statisticsService.GetCachedVisitors());
        if (!visitors.Any())
            return NotFound();
        JsonDocument doc = JsonDocument.Parse("{\"visitors\":" + JsonSerializer.Serialize(visitors, new JsonSerializerOptions{ 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }) + "}");
        JsonElement json = doc.RootElement;
        var generator = new VueStartGenerator(json, memoryCache);
        string content = "";
        string contentType;
        string artifactId = generateService.Generate(json, "Dashboard", "bootstrap-table.sbn", "bootstrap", "42b983", generator.Id, true, out string appjs, out string indexhtml, true);
        
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "index.html";
            
        switch (fileName.Split('.').LastOrDefault())
        {
            case "js":
                contentType = "application/javascript";
                content = appjs;
            break;
            case "html":
                contentType = "text/html";
                content = indexhtml;
            break;
            default:
                contentType = "text/plain";
            break;
        }
        var cd = new System.Net.Mime.ContentDisposition
        {
                FileName = fileName,
                Inline = true
        };
        Response.Headers.Add("Content-Disposition", cd.ToString());
        Response.Headers.Add("X-Content-Type-Options", "nosniff");
        return File(Encoding.UTF8.GetBytes(content), contentType);
    }
}
