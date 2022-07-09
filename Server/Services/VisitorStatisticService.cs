using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UAParser;

namespace VueStart.Services;

public class VisitorStatisticService
{
    private Dictionary<string, VisitorData> Visitors { get; } = new Dictionary<string, VisitorData>();
    private readonly GeoLocationService geoLocationService;
    private readonly ILogger<VisitorStatisticService> logger;

    public VisitorStatisticService(GeoLocationService geoLocationService, ILogger<VisitorStatisticService> logger)
    {
        this.geoLocationService = geoLocationService;
        this.logger = logger;
    }

    public void StoreVisit(EventData eventData)
    {
        if (Visitors.TryGetValue(eventData.IdToken, out var data))
        {
            var visit = data.Visitor.Visits.First();
            visit.Count += 1;
            visit.End = DateTime.UtcNow;
        }
        else
        {
            var visitor = CreateVisitor(eventData.UaString, eventData.IdToken, eventData.Citation);
            Visitors.Add(eventData.IdToken, new VisitorData {
                Visitor = visitor,
                Ip = eventData.RemoteIpAddress
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

    public async Task SaveVisitors(ApplicationDbContext dbContext)
    {
        var toLocate = new List<VisitorData>();
        logger.Log(LogLevel.Information, $"Saving {Visitors.Count} visitors.");
        foreach (var item in Visitors)
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
            await geoLocationService.SetGeoLocation(toLocate);
            foreach (var item in toLocate) {
                dbContext.Visitors.Add(item.Visitor);
            }
        }
        Visitors.Clear();
    }

    internal IEnumerable<Visitor> GetCachedVisitors()
    {
        return Visitors.Values.Select(d => d.Visitor);
    }

    private Visitor CreateVisitor(string uaString, string token, string citation)
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
        logger.Log(LogLevel.Information, "New visitor");
        return visitor;
    }
}
