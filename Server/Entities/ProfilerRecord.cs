
using System;

namespace VueStart
{
    public class ProfilerRecord
    {
        public int Id { get; set; }
        public int Day { get; set; }
        public int Period { get; set; }
        public int Count { get; set; }
        public long Database { get; set; }
        public long Generate { get; set; }
        public long Download { get; internal set; }
        public long GeoLocation { get; set; }
    }
}