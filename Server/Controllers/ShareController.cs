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
    [Route("")]
    public IActionResult Save([FromBody] JsonElement json)
    {
        int hash = StringHash(JsonSerializer.Serialize(json));
        var link = dbContext.ShareableLinks.FirstOrDefault(r => r.Hash == hash);
        if(link != null)
            return Ok(new { hash = hash });
        dbContext.ShareableLinks.Add(new ShareableLink { Hash = hash, Json = json });
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
        return Ok(link);
    }
}