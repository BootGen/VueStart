using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VueStart
{
    public class Visitor {

        public int Id { get; set; }
        public string Token { get; init; }
        public string Citation { get; init; }
        public DateTime FirstVisit { get; init; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        [JsonIgnore]
        public string UserAgent { get; set; }
        public string OSFamily { get; set; }
        public string OSMajor { get; set; }
        public string OSMinor { get; set; }
         public string DeviceFamily { get; set; }
         public string DeviceBrand { get; set; }
         public string BrowserFamily { get; set; }
         public string BrowserMajor { get; set; }
         public string BrowserMinor { get; set; }
         public string DeviceModel { get; set; }
        public List<Visit> Visits { get; set; }
    }
}