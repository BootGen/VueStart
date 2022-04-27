using System.Linq;
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
    [Route("{json}")]
    public IActionResult Save([FromBody] string json)
    {
        int hash = StringHash(json);
        dbContext.ShareableLinks.Add(new ShareableLink { Hash = hash, Json = json });
        dbContext.SaveChanges();
        return Ok("https://vuestart.com/" + hash);
    }

    [HttpPost]
    [Route("{hash}")]
    public IActionResult Load([FromBody] int hash)
    {
        var link = dbContext.ShareableLinks.FirstOrDefault(r => r.Hash == hash);
        if (link == null)
            return NotFound();
        return Ok(link);
    }
}