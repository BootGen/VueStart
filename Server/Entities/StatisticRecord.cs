
using System;

namespace VueStart
{
    public class StatisticRecord
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public string Data { get; set; }
        public int CardGeneratedCount { get; set; }
        public int CardDownloadedCount { get; set; }
        public int TableGeneratedCount { get; set; }
        public int TableDownloadedCount { get; set; }
        public int WizardGeneratedCount { get; set; }
        public int WizardDownloadedCount { get; set; }
        public DateTime FirstUse { get; set; }
        public DateTime LastUse { get; set; }
    }
}