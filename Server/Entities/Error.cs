namespace VueStart
{
    public class Error
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; internal set; }
        public string File { get; internal set; }
        public int Line { get; internal set; }
        public string Source { get; internal set; }
        public int HResult { get; internal set; }
        public string Data { get; internal set; }
    }
}