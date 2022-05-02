
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace VueStart
{

    public class InputData
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public JsonElement Data { get; set; }
        public DateTime FirstUse { get; set; }
        public DateTime LastUse { get; set; }
        public bool Error { get; set; }
        public List<StatisticRecord> StatisticRecords { get; set; }
    }
}