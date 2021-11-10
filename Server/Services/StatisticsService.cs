using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using UAParser;

namespace VueStart.Services
{
    public enum ActionType {
        Generate,
        Download
    }

    public enum ArtifactType {
        View,
        Form,
        Editor
    }

    public class StatisticsService
    {
        private readonly IConfiguration configuration;

        public StatisticsService(IConfiguration configuration)
        {
            this.configuration = configuration;
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

        private void UpdateRecord(StatisticRecord record, ActionType actionType, ArtifactType artifactType)
        {
            record.LastUse = DateTime.Now;
            switch (actionType)
            {
                case ActionType.Download:
                switch (artifactType)
                {
                    case ArtifactType.View:
                    record.ViewDownloadedCount += 1;
                    break;
                    case ArtifactType.Form:
                    record.FormDownloadedCount += 1;
                    break;
                    case ArtifactType.Editor:
                    record.EditorDownloadedCount += 1;
                    break;
                }
                break;
                case ActionType.Generate:
                switch (artifactType)
                {
                    case ArtifactType.View:
                    record.ViewGeneratedCount += 1;
                    break;
                    case ArtifactType.Form:
                    record.FormGeneratedCount += 1;
                    break;
                    case ArtifactType.Editor:
                    record.EditorGeneratedCount += 1;
                    break;
                }
                break;
            }
        }

        public  async void  onEvent(HttpContext context, string data, ActionType actionType, ArtifactType artifactType)
        {
            string uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            await Task.Run(async () => {
                int hash = StringHash(data);
                using (var dbContext = new ApplicationDbContext(configuration))
                {
                    dbContext.Database.EnsureCreated();
                    if (!string.IsNullOrWhiteSpace(token))
                        await LogVisit(uaString, remoteIpAddress, token, dbContext);
                    var record = dbContext.StatisticRecords.Where(r => r.Hash == hash && r.Data == data).FirstOrDefault();
                    if (record == null)
                    {
                        record = new StatisticRecord
                        {
                            Data = data,
                            Hash = hash,
                            FirstUse = DateTime.Now
                        };
                        UpdateRecord(record, actionType, artifactType);
                        dbContext.StatisticRecords.Add(record);
                    } else {
                        UpdateRecord(record, actionType, artifactType);
                    }
                    dbContext.SaveChanges();
                }        
            });
        }

        

        private async Task LogVisit(string uaString, string remoteIpAddress, string token, ApplicationDbContext dbContext)
        {
            var visitor = dbContext.Visitors.FirstOrDefault(v => v.Token == token);
            if (visitor == null)
            {
                visitor = CreateVisitor(uaString, token);
                await SetGeoLocation(remoteIpAddress, visitor);
                visitor = dbContext.Visitors.Add(visitor).Entity;
            }
            AddVisit(dbContext, visitor);
        }

        private void AddVisit(ApplicationDbContext dbContext, Visitor visitor)
        {
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
        }

        private async Task SetGeoLocation(string remoteIpAddress, Visitor visitor)
        {
            var ipInfotoken = configuration.GetValue<string>("IpInfoToken");
            if (string.IsNullOrWhiteSpace(remoteIpAddress) || string.IsNullOrWhiteSpace(ipInfotoken))
                return;
            var geoLocationTask = WebRequest.Create($"https://ipinfo.io/{remoteIpAddress}?token={ipInfotoken}").GetResponseAsync();
            using (var response = await geoLocationTask)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var jsonString = reader.ReadToEnd();
                    var jObject = JObject.Parse(jsonString);
                    visitor.Country = jObject.GetValue("country")?.ToString();
                    visitor.Region = jObject.GetValue("region")?.ToString();
                    visitor.City = jObject.GetValue("city")?.ToString();
                }
            }
        }

        private static Visitor CreateVisitor(string uaString, string token)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);
            var visitor = new Visitor
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
            return visitor;
        }
    }
}