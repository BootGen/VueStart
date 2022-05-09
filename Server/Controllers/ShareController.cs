using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace VueStart.Controllers;

[ApiController]
[Route("api/share")]
public class ShareController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;

    public ShareController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    private static int StringHash(string text)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in text)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    
    [HttpPost]
    [Route("{type}/{editable}/{color}")]
    public IActionResult Save([FromBody] JsonElement json, string type, bool editable, string color)
    {
        int hash = StringHash(JsonSerializer.Serialize(json)+type+editable.ToString()+color);
        var link = dbContext.ShareableLinks.FirstOrDefault(r => r.Hash == hash);
        if(link != null) {
            return Ok(new { hash = hash });
        }
        dbContext.ShareableLinks.Add(new ShareableLink { Hash = hash, Json = json, FrontendType = type, Editable = editable, Color = color, FirstUse = DateTime.UtcNow, LastUse = DateTime.UtcNow, Count = 0});
        dbContext.SaveChanges();
        return Ok(new { hash = hash });
    }

    [HttpGet]
    [Route("{hash}")]
    public IActionResult Load([FromRoute] int hash)
    {
        var link = dbContext.ShareableLinks.FirstOrDefault(r => r.Hash == hash);
        if (link == null)
            return NotFound();
        link.LastUse = DateTime.UtcNow;
        link.Count += 1;
        dbContext.SaveChanges();
        return Ok(link);
    }
}