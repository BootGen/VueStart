
using System;

namespace VueStart
{
    public class ProfilerRecord
    {
        public int Id { get; set; }
        public int Day { get; init; }
        public int Period { get; init; }
        public int Count { get; set; }
        public long Database { get; set; }
        public long Generate { get; set; }
        public long Download { get; set; }
        public long GeoLocation { get; set; }
    }
}