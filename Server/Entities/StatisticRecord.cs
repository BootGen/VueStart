
using System;

namespace VueStart
{
    public class StatisticRecord
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public string Data { get; set; }
        public int BootstrapReadonlyGeneratedCount { get; set; }
        public int BootstrapEditableGeneratedCount { get; set; }
        public int BootstrapReadonlyDownloadedCount { get; set; }
        public int BootstrapEditableDownloadedCount { get; set; }
        public int TailwindReadonlyGeneratedCount { get; set; }
        public int TailwindEditableGeneratedCount { get; set; }
        public int TailwindReadonlyDownloadedCount { get; set; }
        public int TailwindEditableDownloadedCount { get; set; }
        public int VanillaReadonlyGeneratedCount { get; set; }
        public int VanillaEditableGeneratedCount { get; set; }
        public int VanillaReadonlyDownloadedCount { get; set; }
        public int VanillaEditableDownloadedCount { get; set; }
        public DateTime FirstUse { get; set; }
        public DateTime LastUse { get; set; }
    }
}