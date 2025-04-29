namespace FinCtrlLibrary.Exceptions
{
    public class AppSettingsValueNotFoundException : Exception
    {
        public AppSettingsValueNotFoundException() : base() { }
        public AppSettingsValueNotFoundException(string message) : base(message) { }
        public AppSettingsValueNotFoundException(string message, Exception e) : base(message, e) { }
    }
}
