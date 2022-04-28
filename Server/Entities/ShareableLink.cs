using System;
using System.Text.Json;

namespace VueStart
{
    public class ShareableLink {
        public int Id { get; set; }
        public int Hash { get; set; }
        public JsonElement Json { get; set; }
    }
}