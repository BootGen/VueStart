using System;

namespace VueStart
{
    public class ClientError
    {
        public int Id { get; set; }
        public DateTime DateTime { get; init; }
        public string UserAgent { get; set; }
        public string Data { get; set; }
    }
}