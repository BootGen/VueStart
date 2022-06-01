using System;
using System.Text.Json;

namespace VueStart
{
    public class ShareableLink {
        public int Id { get; set; }
        public int Hash { get; set; }
        public JsonElement GenerateRequest { get; set; }
        public DateTime FirstUse { get; set; }
        public DateTime LastUse { get; set; }
        public int Count { get; set; }
    }
}