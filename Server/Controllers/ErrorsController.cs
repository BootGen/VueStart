using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BootGen;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/errors")]
    public class ErrorsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ErrorsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("")]
        public IActionResult LogError([FromBody] Error error) {
            using var dbContext = new ApplicationDbContext(configuration);
            dbContext.Errors.Add(error);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
