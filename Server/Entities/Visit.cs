using System;

namespace VueStart
{
    public class Visit {
        public int Id { get; set; }
        public DateTime Start { get; init; }
        public DateTime End { get; set; }
        public int Count { get; set; }
    }
}