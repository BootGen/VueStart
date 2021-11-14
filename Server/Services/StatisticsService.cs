using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
        None,
        View,
        Form,
        Editor
    }

    public class StatisticsService
    {
        private struct CacheKey
        {
            public int Day { get; set; }
            public int Period { get; set; }
        }

        private struct VisitorData
        {
            public Visitor Visitor { get; set; }
            public string Ip { get; set; }
        }

        private struct PeriodData
        {
            public Dictionary<string, VisitorData> Visitors;
            public List<StatisticRecord> Records;
            public ProfilerRecord ProfilerRecord;
        }

        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        public StatisticsService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
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

        public void OnEvent(HttpContext context, string jsonData, ActionType actionType, ArtifactType artifactType)
        {   
            var now = DateTime.Now;
            var periodLengthInMinutes = 15;
            var key = new CacheKey
            {
                Day = (now - new DateTime(2021, 1, 1)).Days,
                Period = (int)now.TimeOfDay.TotalMinutes / periodLengthInMinutes
            };
            var periodData = memoryCache.GetOrCreate(key, entry => {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(periodLengthInMinutes + 0.5));
                entry.RegisterPostEvictionCallback( (object key, object value, EvictionReason reason, object state) => {
                    Task.Run(async () => {
                        await SaveData((PeriodData)value);
                    });
                });
                return new PeriodData
                {
                    Visitors = new Dictionary<string, VisitorData>(),
                    Records = new List<StatisticRecord>(),
                    ProfilerRecord = new ProfilerRecord
                    {
                        Day = key.Day,
                        Period = key.Period
                    }
                };
            });
            periodData.ProfilerRecord.Count += 1;
            SaveVisitToCahce(periodData.Visitors, context, key);
            SaveStatisticRecordToCache(periodData.Records, jsonData, actionType, artifactType);
        }


        private void SaveStatisticRecordToCache(List<StatisticRecord> records, string jsonData, ActionType actionType, ArtifactType artifactType)
        {
            int hash = StringHash(jsonData);
            var record = records.FirstOrDefault(r => r.Hash == hash);
            if (record == null)
            {
                record = new StatisticRecord
                {
                    Hash = hash,
                    Data = jsonData,
                    FirstUse = DateTime.Now
                };
                records.Add(record);
            }
            UpdateRecord(record, actionType, artifactType);
        }

        private void SaveVisitToCahce(Dictionary<string, VisitorData> visitors, HttpContext context, CacheKey key)
        {
            string uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            string citation = context.Request.Headers["citation"].FirstOrDefault();
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            if (visitors.TryGetValue(token, out var data))
            {
                data.Visitor.Visits.First().Count += 1;
            }
            else
            {
                var visitor = CreateVisitor(uaString, token, citation);
                visitors.Add(token, new VisitorData {
                    Visitor = visitor,
                    Ip = remoteIpAddress
                });
                visitor.Visits = new List<Visit> {
                    new Visit
                    {
                        Day = key.Day,
                        Period = key.Period,
                        Count = 1
                    }
                };
            }
        }

        private async Task SaveData(PeriodData data)
        {
            var sw = new Stopwatch();
            sw.Start();
            using (var dbContext = new ApplicationDbContext(configuration))
            {
                dbContext.Database.EnsureCreated();
                await SaveVisitors(data, dbContext);
                SaveRecords(data.Records, dbContext);
                dbContext.SaveChanges();
                sw.Stop();
                data.ProfilerRecord.Database = sw.ElapsedMilliseconds - data.ProfilerRecord.GeoLocation;
                sw.Restart();
                dbContext.ProfilerRecords.Add(data.ProfilerRecord);
                dbContext.SaveChanges();
                sw.Stop();
                Console.WriteLine($"\n\nSaving profiler record: {sw.ElapsedMilliseconds}");
            }
        }

        private static void SaveRecords(List<StatisticRecord> records, ApplicationDbContext dbContext)
        {
            foreach (var record in records)
            {
                var existingRecord = dbContext.StatisticRecords.FirstOrDefault(r => r.Hash == record.Hash);
                if (existingRecord != null)
                {
                    existingRecord.ViewGeneratedCount += record.ViewGeneratedCount;
                    existingRecord.ViewDownloadedCount += record.ViewDownloadedCount;
                    existingRecord.FormGeneratedCount += record.FormGeneratedCount;
                    existingRecord.FormDownloadedCount += record.FormDownloadedCount;
                    existingRecord.EditorGeneratedCount += record.EditorGeneratedCount;
                    existingRecord.EditorDownloadedCount += record.EditorDownloadedCount;
                    existingRecord.LastUse = record.LastUse;
                }
                else
                {
                    dbContext.StatisticRecords.Add(record);
                }
            }
        }

        private async Task SaveVisitors(PeriodData data, ApplicationDbContext dbContext)
        {
            var toLocate = new List<VisitorData>();
            foreach (var item in data.Visitors)
            {
                var visitor = item.Value.Visitor;
                var earlierVisitor = dbContext.Visitors.FirstOrDefault(v => v.Token == visitor.Token);
                if (earlierVisitor != null)
                {
                    dbContext.Entry(earlierVisitor).Collection(v => v.Visits).Load();
                    earlierVisitor.Visits.AddRange(visitor.Visits);
                }
                else
                {
                    toLocate.Add(item.Value);
                }
            }
            if (toLocate.Any()) {
                var sw = new Stopwatch();
                sw.Start();
                await SetGeoLocation(toLocate);
                sw.Stop();
                data.ProfilerRecord.GeoLocation = sw.ElapsedMilliseconds;
                foreach (var item in toLocate) {
                    dbContext.Visitors.Add(item.Visitor);
                }
            }
        }

        private async Task SetGeoLocation(List<VisitorData> data)
        {
            var ipInfotoken = configuration.GetValue<string>("IpInfoToken");
            if (string.IsNullOrWhiteSpace(ipInfotoken))
                return;
            WebRequest webRequest = WebRequest.Create($"https://ipinfo.io/batch?token={ipInfotoken}");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            string ipListString = data.Select(d => $"\"{d.Ip}\"").Aggregate((a, b) => $"{a}, {b}");
            string stringData = $"[{ipListString}]";
            var body = Encoding.Default.GetBytes(stringData);
            webRequest.GetRequestStream().Write(body, 0, body.Length);
            var geoLocationTask = webRequest.GetResponseAsync();
            using (var response = await geoLocationTask)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var jsonString = reader.ReadToEnd();
                    var jObject = JObject.Parse(jsonString);
                    foreach (var item in data) {
                        var obj = jObject.GetValue(item.Ip) as JObject;
                        if (obj != null) {
                            SetLocation(item.Visitor, obj);
                            Console.WriteLine("\n\nGeo location:");
                            Console.WriteLine(JObject.FromObject(item.Visitor).ToString());
                        }
                    }
                }
            }
        }

        private static void SetLocation(Visitor visitor, JObject jObject)
        {
            visitor.Country = jObject.GetValue("country")?.ToString();
            visitor.Region = jObject.GetValue("region")?.ToString();
            visitor.City = jObject.GetValue("city")?.ToString();
        }

        private static Visitor CreateVisitor(string uaString, string token, string citation)
        {
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(uaString);
            var visitor = new Visitor
            {
                Token = token,
                Citation = citation,
                FirstVisit = DateTime.Now,
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
            Console.WriteLine("\n\nNew visitor:");
            Console.WriteLine(JObject.FromObject(visitor).ToString());
            return visitor;
        }
    }
}