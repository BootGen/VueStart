
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VueStart
{

    public class StatisticRecord
    {
        public int Id { get; set; }
        public int InputDataId { get; set; }
        
        [JsonIgnore]
        public InputData InputData { get; set; }
        public bool Readonly { get; set; }
        public bool Download { get; set; }
        public int BootstrapCount { get; set; }
        public int TailwindCount { get; set; }
        public int VanillaCount { get; set; }

        internal bool IsSameKind(StatisticRecord record)
        {
            return Readonly == record.Readonly && Download == record.Download;
        }
    }
}