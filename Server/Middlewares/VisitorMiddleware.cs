
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StartVue.Middlewares
{
    public class VisitorMiddleware
    {
        private readonly RequestDelegate _next;

        public VisitorMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            if (!string.IsNullOrEmpty(token)) {
                var visitor = dbContext.Visitors.FirstOrDefault(v => v.Token == token);
                if (visitor == null) {
                    visitor = new Visitor {
                        Token = token,
                        CountryCode = "@"
                    };
                    dbContext.Visitors.Add(visitor);
                }
                visitor.Visits.Add(new Visit {
                    Time = DateTime.Now
                });
                dbContext.SaveChanges();
            }
            await _next(context);
        }
    }
}