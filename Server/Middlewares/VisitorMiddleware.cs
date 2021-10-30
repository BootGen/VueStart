
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using UAParser;

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
                dbContext.Database.EnsureCreated();
                var visitor = dbContext.Visitors.FirstOrDefault(v => v.Token == token);
                if (visitor == null) {
                    var uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
                    var uaParser = Parser.GetDefault();
                    ClientInfo c = uaParser.Parse(uaString);
                    visitor = new Visitor {
                        Token = token,
                        CountryCode = "@",
                        UserAgent = uaString,
                        OSFamily = c.OS.Family,
                        OSMajor = c.OS.Major,
                        OSMinor = c.OS.Minor,
                        DeviceBrand = c.Device.Brand,
                        DeviceFamily = c.Device.Family,
                        DeviceModel = c.Device.Model,
                        BrowserFamily = c.UA.Family,
                        BrowserMajor = c.UA.Major,
                        BrowserMinor = c.UA.Minor
                    };
                    visitor = dbContext.Visitors.Add(visitor).Entity;
                }
                dbContext.Entry(visitor).Collection(v => v.Visits).Load();
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