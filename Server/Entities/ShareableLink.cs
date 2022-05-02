using System;
using System.Text.Json;

namespace VueStart
{
    public class ShareableLink {
        public int Id { get; set; }
        public int Hash { get; set; }
        public JsonElement Json { get; set; }
        public string FrontendType { get; set; }
        public bool Editable { get; set; }
        public string Color { get; set; }
        public DateTime FirstUse { get; set; }
        public DateTime LastUse { get; set; }
        public int Count { get; set; }
    }
}