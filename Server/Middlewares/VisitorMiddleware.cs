
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
        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            if (!string.IsNullOrEmpty(token))
            {
                await LogVisit(context, serviceProvider, token);
            }
            await _next(context);
        }

        private static async Task LogVisit(HttpContext context, IServiceProvider serviceProvider, string token)
        {
            var dbContext = serviceProvider.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var config = serviceProvider.GetService(typeof(IConfiguration)) as IConfiguration;
            dbContext.Database.EnsureCreated();
            var visitor = dbContext.Visitors.FirstOrDefault(v => v.Token == token);
            if (visitor == null)
            {
                var ipInfotoken = config.GetValue<string>("IpInfoToken");
                if (context.Connection.RemoteIpAddress != null && !string.IsNullOrWhiteSpace(ipInfotoken))
                {
                    var geoLocationTask = WebRequest.Create($"https://ipinfo.io/{context.Connection.RemoteIpAddress?.ToString()}?token={ipInfotoken}").GetResponseAsync();
                    if (geoLocationTask != null && visitor != null)
                    {
                        using (var response = await geoLocationTask)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
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
                var uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
                var uaParser = Parser.GetDefault();
                ClientInfo c = uaParser.Parse(uaString);
                visitor = new Visitor
                {
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
            var today = DateTime.Now.Date;
            var visit = visitor.Visits.FirstOrDefault(v => v.Start.Date == today);
            if (visit != null)
            {
                visit.Count += 1;
                visit.End = DateTime.Now;
            }
            else
            {
                visit = new Visit
                {
                    Start = DateTime.Now,
                    End = DateTime.Now,
                    Count = 1
                };
                visitor.Visits.Add(visit);
            }
            Console.WriteLine(JObject.FromObject(visit).ToString());
            dbContext.SaveChanges();
        }
    }
}