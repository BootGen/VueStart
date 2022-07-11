using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace VueStart.Services;

public class GeoLocationService
{
    private readonly IConfiguration configuration;

    public GeoLocationService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task SetGeoLocation(List<VisitorData> data)
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
            }
        }
    }

    private static void SetLocation(Visitor visitor, JObject jObject)
    {
        visitor.Country = jObject.GetValue("country")?.ToString();
        visitor.Region = jObject.GetValue("region")?.ToString();
        visitor.City = jObject.GetValue("city")?.ToString();
    }
}