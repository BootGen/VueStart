
using System;

namespace VueStart
{
    public class StatisticRecord
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public string Data { get; set; }
        public int ViewGeneratedCount { get; set; }
        public int ViewDownloadedCount { get; set; }
        public int FormGeneratedCount { get; set; }
        public int FormDownloadedCount { get; set; }
        public int EditorGeneratedCount { get; set; }
        public int EditorDownloadedCount { get; set; }
        public DateTime FirstUse { get; set; }
        public DateTime LastUse { get; set; }
    }
}