using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.IO.Compression;
using VueStart.Services;
using VueStart.Data;
using System.Text;
using BootGen;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace VueStart.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMemoryCache memoryCache;
    private readonly GenerationService generationService;
    private readonly GenerationService generateService;
    private readonly StatisticsService statisticsService;

    public AdminController(ApplicationDbContext dbContext, IMemoryCache memoryCache, GenerationService generationService, GenerationService generateService, StatisticsService statisticsService)
    {
        this.dbContext = dbContext;
        this.memoryCache = memoryCache;
        this.generationService = generationService;
        this.generateService = generateService;
        this.statisticsService = statisticsService;
    }

    [HttpGet]
    [Route("visitors")]
    public JsonElement GetVisitors() {
        List<Visitor> visitors = dbContext.Visitors.Include(visitor => visitor.Visits).ToList();
        visitors.AddRange(statisticsService.GetCachedVisitors());
        JsonDocument doc = JsonDocument.Parse("{\"visitors\":" + JsonSerializer.Serialize(visitors, new JsonSerializerOptions{ 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }) + "}");
        JsonElement root = doc.RootElement;
        return root;
    }

    [HttpGet]
    [Route("{fileName}")]
    public IActionResult GetTable(string fileName)
    {
        
        var generator = new VueStartGenerator(GetVisitors(), memoryCache);
        string content = "";
        string contentType;
        string artifactId = generateService.Generate(GetVisitors(), "Data Table", "bootstrap-table.sbn", "bootstrap", "42b983", generator.Id, true, out string appjs, out string indexhtml, true);
        
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
