namespace WebLibrary.Exceptions
{
    public class HttpContentParseException : Exception
    {
        public HttpContentParseException() : base() { }
        public HttpContentParseException(string message) : base(message) { }
        public HttpContentParseException(string message, Exception e) : base(message, e) { }
    }
}
