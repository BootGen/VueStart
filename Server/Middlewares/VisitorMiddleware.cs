
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VueStart.Middlewares
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
                var now = DateTime.Now;
                var date = new DateTime(now.Year, now.Month, now.Day);
                var visit = visitor.Visits.FirstOrDefault(v => v.Date == date);
                if (visit != null) {
                    visit.Count += 1;
                } else {
                    visitor.Visits.Add(new Visit {
                        Date = date,
                        Count = 1
                    });
                }
                dbContext.SaveChanges();
            }
            await _next(context);
        }
    }
}