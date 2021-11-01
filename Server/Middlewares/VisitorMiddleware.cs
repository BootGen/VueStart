
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, IConfiguration config)
        {
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            Task<WebResponse> geoLocationTask = null;
            Visitor visitor = null;
            if (!string.IsNullOrEmpty(token)) {
                dbContext.Database.EnsureCreated();
                visitor = dbContext.Visitors.FirstOrDefault(v => v.Token == token);
                if (visitor == null) {
                    var ipInfotoken = config.GetValue<string>("IpInfoToken");
                    if (context.Connection.RemoteIpAddress != null && !string.IsNullOrWhiteSpace(ipInfotoken)) 
                        geoLocationTask = WebRequest.Create($"https://ipinfo.io/{context.Connection.RemoteIpAddress?.ToString()}?token={ipInfotoken}").GetResponseAsync();
                    var uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
                    var uaParser = Parser.GetDefault();
                    ClientInfo c = uaParser.Parse(uaString);
                    visitor = new Visitor {
                        Token = token,
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
            if (geoLocationTask != null && visitor != null) {
                using (var response = await geoLocationTask) {
                    using (var reader = new StreamReader(response.GetResponseStream())) {
                        var jsonString = reader.ReadToEnd();
                        var jObject = JObject.Parse(jsonString);
                        visitor.Country = jObject.GetValue("country")?.ToString();
                        visitor.Region = jObject.GetValue("region")?.ToString();
                        visitor.City = jObject.GetValue("city")?.ToString();
                        dbContext.Update(visitor);
                        dbContext.SaveChanges();
                    }
                }
            }
        }

       
    }
}