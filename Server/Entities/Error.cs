namespace VueStart
{
    public class Error
    {
        public int Id { get; set; }
        public string Message { get; init; }
        public string StackTrace { get; init; }
        public string File { get; init; }
        public int Line { get; init; }
        public string Source { get; init; }
        public int HResult { get; init; }
        public string Data { get; set; }
    }
}