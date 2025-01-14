namespace FinCtrlLibrary.Exceptions
{
    public class InfinityValueException : Exception
    {
        public InfinityValueException() : base() { }
        public InfinityValueException(string message) : base(message) { }
        public InfinityValueException(string message, Exception e) : base(message, e) { }
    }
}
