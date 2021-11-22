using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System;

namespace VueStart.Controllers
{
    [ApiController]
    [Route("api/errors")]
    public class ClientErrorsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ClientErrorsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("")]
        public IActionResult LogError([FromBody] JsonElement data) {
            Console.WriteLine( data.ToString());
            using var dbContext = new ApplicationDbContext(configuration);
            dbContext.Database.EnsureCreated();
            dbContext.ClientErrors.Add(new ClientError{
                DateTime = DateTime.Now,
                UserAgent = Request.Headers["User-Agent"].FirstOrDefault(),
                Data = data.ToString()
            });
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
