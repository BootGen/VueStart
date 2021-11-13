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

        public void OnEvent(HttpContext context, string data, ActionType actionType, ArtifactType artifactType)
        {   
            var now = DateTime.Now;
            int day = (now - new DateTime(2021, 1, 1)).Days;
            int period = (int)now.TimeOfDay.TotalMinutes / 15;
            int prevPeriod = memoryCache.GetOrCreate("period", e => period);
            var visitors = memoryCache.GetOrCreate("visitors", e => new Dictionary<string, Tuple<Visitor, string>>());
            var records = memoryCache.GetOrCreate("records", e => new List<StatisticRecord>());
            if (period != prevPeriod) {
                memoryCache.Set("visitors", new Dictionary<string, Tuple<Visitor, string>>());
                memoryCache.Set("records", new List<StatisticRecord>());
            }
            SaveVisitToCahce(visitors, context, day, period);
            SaveStatisticRecordToCache(records, data, actionType, artifactType);
            if (period != prevPeriod) {
                Task.Run(async () =>
                {
                        var sw = new Stopwatch();
                        sw.Start();
                        await SaveData(visitors, records);
                        sw.Stop();
                        Console.WriteLine($"Saving time: {sw.ElapsedMilliseconds}");
                });
            }
        }


        private void SaveStatisticRecordToCache(List<StatisticRecord> records, string data, ActionType actionType, ArtifactType artifactType)
        {
            int hash = StringHash(data);
            var record = records.FirstOrDefault(r => r.Hash == hash);
            if (record == null)
            {
                record = new StatisticRecord
                {
                    Hash = hash,
                    Data = data
                };
                records.Add(record);
            }
            UpdateRecord(record, actionType, artifactType);
        }

        private void SaveVisitToCahce(Dictionary<string, Tuple<Visitor, string>> visitors, HttpContext context, int day, int period)
        {
            string uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            if (visitors.TryGetValue(token, out var data))
            {
                data.Item1.Visits.First().Count += 1;
            }
            else
            {
                var visitor = CreateVisitor(uaString, token);
                visitors.Add(token, Tuple.Create(visitor, remoteIpAddress));
                visitor.Visits = new List<Visit>{
                    new Visit
                    {
                        Day = day,
                        Period = period,
                        Count = 1
                    }
                };
            }
        }

        private async Task SaveData(Dictionary<string, Tuple<Visitor, string>> visitors, List<StatisticRecord> records)
        {
            using (var dbContext = new ApplicationDbContext(configuration))
            {
                dbContext.Database.EnsureCreated();
                await SaveVisitors(visitors, dbContext);
                SaveRecords(records, dbContext);
                dbContext.SaveChanges();
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

        private async Task SaveVisitors(Dictionary<string, Tuple<Visitor, string>> visitors, ApplicationDbContext dbContext)
        {
            var toLocate = new List<Tuple<Visitor, string>>();
            foreach (var item in visitors)
            {
                var visitor = item.Value.Item1;
                var earlierVisitor = dbContext.Visitors.FirstOrDefault(v => v.Token == visitor.Token);
                if (earlierVisitor != null)
                {
                    dbContext.Entry(earlierVisitor).Collection(v => v.Visits).Load();
                    earlierVisitor.Visits.AddRange(visitor.Visits);
                }
                else
                {
                    visitor = dbContext.Visitors.Add(visitor).Entity;
                    toLocate.Add(Tuple.Create(visitor, item.Value.Item2));
                }
            }
            if (toLocate.Any()) {
                var sw = new Stopwatch();
                sw.Start();
                await SetGeoLocation(toLocate);
                sw.Stop();
                Console.WriteLine($"Geo location time: {sw.ElapsedMilliseconds}");
            }
        }

        private async Task SetGeoLocation(List<Tuple<Visitor, string>> data)
        {
            var ipInfotoken = configuration.GetValue<string>("IpInfoToken");
            if (string.IsNullOrWhiteSpace(ipInfotoken))
                return;
            WebRequest webRequest = WebRequest.Create($"https://ipinfo.io/batch?token={ipInfotoken}");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            string ipListString = data.Select(d => $"\"{d.Item2}\"").Aggregate((a, b) => $"{a}, {b}");
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
                        var obj = jObject.GetValue(item.Item2) as JObject;
                        if (obj != null)
                            SetLocation(item.Item1, obj);
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