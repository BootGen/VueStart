using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using UAParser;
using VueStart.Data;

namespace VueStart.Services
{

    public class StatisticsService
    {
        private struct CacheKey
        {
            public int Day { get; init; }
            public int Period { get; init; }
        }

        private struct VisitorData
        {
            public Visitor Visitor { get; init; }
            public string Ip { get; init; }
        }

        private struct PeriodData
        {
            public Dictionary<string, VisitorData> Visitors { get; init; }
            public List<InputData> InputData { get; init; }
            public ProfilerRecord ProfilerRecord { get; init; }
        }

        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        private Stopwatch GenerateWatch;
        private PeriodData Data;

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

        private void UpdateRecord(StatisticRecord record,  CssType cssType)
        {
            switch (cssType)
            {
                case CssType.Bootstrap:
                    record.BootstrapCount += 1;
                break;
                case CssType.Tailwind:
                    record.TailwindCount += 1;
                break;
                case CssType.Vanilla:
                    record.VanillaCount += 1;
                break;
            }
        }

        public void OnGenerateEnd()
        {
            GenerateWatch.Stop();
            Data.ProfilerRecord.Generate += GenerateWatch.ElapsedMilliseconds;
        }
        public void OnDownloadEnd()
        {
            GenerateWatch.Stop();
            Data.ProfilerRecord.Download += GenerateWatch.ElapsedMilliseconds;
        }

        public void OnEvent(HttpContext context, JsonElement jsonData, ActionType actionType, ArtifactType artifactType, CssType cssType, bool error = false)
        {
            CacheKey key = SetCurrentCacheEntry();
            Data.ProfilerRecord.Count += 1;
            SaveVisitToCahce(Data.Visitors, context, key);
            SaveStatisticRecordToCache(Data, jsonData, actionType, artifactType, cssType, error);
            GenerateWatch = new Stopwatch();
            GenerateWatch.Start();
        }

        public List<Visitor> GetCachedVisitors()
        {
            SetCurrentCacheEntry();
            return Data.Visitors.Select(kvp => kvp.Value.Visitor).ToList();
        }

        private CacheKey SetCurrentCacheEntry()
        {
            var now = DateTime.UtcNow;
            #if DEBUG
            var periodLengthInMinutes = 1;
            #else
            var periodLengthInMinutes = 15;
            #endif
            var key = new CacheKey
            {
                Day = (now - new DateTime(2021, 1, 1)).Days,
                Period = (int)now.TimeOfDay.TotalMinutes / periodLengthInMinutes
            };
            Data = memoryCache.GetOrCreate(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(periodLengthInMinutes + 0.5));
                entry.RegisterPostEvictionCallback((object key, object value, EvictionReason reason, object state) =>
                {
                    Task.Run(async () =>
                    {
                        await SaveData((PeriodData)value);
                    });
                });
                return new PeriodData
                {
                    Visitors = new Dictionary<string, VisitorData>(),
                    InputData = new List<InputData>(),
                    ProfilerRecord = new ProfilerRecord
                    {
                        Day = key.Day,
                        Period = key.Period
                    }
                };
            });
            return key;
        }

        private void SaveStatisticRecordToCache(PeriodData periodData, JsonElement jsonData, ActionType actionType, ArtifactType artifactType, CssType cssType, bool error)
        {
            int hash = StringHash(jsonData.ToString());
            var inputData = periodData.InputData.FirstOrDefault(r => r.Hash == hash);
            var record = new StatisticRecord {
                Download = actionType == ActionType.Download,
                Readonly = artifactType == ArtifactType.TableEditable
            };
            if (inputData == null)
            {
                inputData = new InputData {
                    Hash = hash,
                    Data = jsonData,
                    FirstUse = DateTime.UtcNow,
                    LastUse = DateTime.UtcNow,
                    Error = error,
                    StatisticRecords = new List<StatisticRecord>() { record }
                };
                periodData.InputData.Add(inputData);
            } else {
                var existingRecord = inputData.StatisticRecords.FirstOrDefault(r => r.IsSameKind(record));
                if (existingRecord != null)
                {
                    record = existingRecord;
                } else {
                    inputData.StatisticRecords.Add(record);
                }
            }
            UpdateRecord(record, cssType);
        }

        private void SaveVisitToCahce(Dictionary<string, VisitorData> visitors, HttpContext context, CacheKey key)
        {
            string uaString = context.Request.Headers["User-Agent"].FirstOrDefault();
            string token = context.Request.Headers["idtoken"].FirstOrDefault();
            string citation = context.Request.Headers["citation"].FirstOrDefault();
            string remoteIpAddress = context.Connection.RemoteIpAddress.ToString();
            if (visitors.TryGetValue(token, out var data))
            {
                var visit = data.Visitor.Visits.First();
                visit.Count += 1;
                visit.End = DateTime.UtcNow;
            }
            else
            {
                var visitor = CreateVisitor(uaString, token, citation);
                visitors.Add(token, new VisitorData {
                    Visitor = visitor,
                    Ip = remoteIpAddress
                });

                visitor.Visits = new List<Visit> {
                    new Visit {
                        Start = DateTime.UtcNow,
                        End = DateTime.UtcNow,
                        Count = 1
                    }
                };
            }
        }

        private async Task SaveData(PeriodData data)
        {
            using var dbContext = new ApplicationDbContext(configuration);
            try {
                var sw = new Stopwatch();
                sw.Start();
                await SaveVisitors(data, dbContext);
                SaveRecords(data, dbContext);
                dbContext.SaveChanges();
                sw.Stop();
                data.ProfilerRecord.Database = sw.ElapsedMilliseconds - data.ProfilerRecord.GeoLocation;
                sw.Restart();
                dbContext.ProfilerRecords.Add(data.ProfilerRecord);
                dbContext.SaveChanges();
                sw.Stop();
                Console.WriteLine($"\n\nSaving profiler record: {sw.ElapsedMilliseconds}");
            } catch (Exception e) {
                using var errorHandlingService = new ErrorHandlerService(dbContext);
                errorHandlingService.OnException(e, null);
                Console.WriteLine(e.Message);
            }
        }

        private void SaveRecords(PeriodData data, ApplicationDbContext dbContext)
        {
            foreach (var inputData in Data.InputData)
            {
                var existingInputData = dbContext.InputData.FirstOrDefault(r => r.Hash == inputData.Hash);
                if (existingInputData != null)
                {
                    existingInputData.LastUse = DateTime.UtcNow;
                    foreach(var record in inputData.StatisticRecords) {
                        var existingRecord = existingInputData.StatisticRecords.FirstOrDefault(r => r.IsSameKind(record));
                        if (existingRecord != null) {
                            existingRecord.BootstrapCount += record.BootstrapCount;
                            existingRecord.TailwindCount += record.TailwindCount;
                            existingRecord.VanillaCount += record.VanillaCount;
                        } else {
                            existingInputData.StatisticRecords.Add(record);
                        }
                    }
                }
                else
                {
                    dbContext.InputData.Add(inputData);
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

            using var client = new HttpClient();
            string ipListString = data.Select(d => $"\"{d.Ip}\"").Aggregate((a, b) => $"{a}, {b}");
            string stringData = $"[{ipListString}]";
            var content = new StringContent(stringData, Encoding.UTF8, "application/json");
                
            using var response = await client.PostAsync($"https://ipinfo.io/batch?token={ipInfotoken}", content);
            using var reader = new StreamReader(response.Content.ReadAsStream());

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
                FirstVisit = DateTime.UtcNow,
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