using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace FinCtrlLibrary.Services
{
    public class LogService
    {
        public static void AddLog(string context, string message)
        {
            string logRegister = $"{context} - {message}";
            Log.Information(logRegister);
        }

        public static void AddLog(string context, string message, object obj)
        {
            string logRegister = $"{context} - {message}";
            logRegister += JsonConvert.SerializeObject(obj, Formatting.Indented);
            Log.Information(logRegister);
        }

        public static void AddLog(string context, string message, string obj)
        {
            string logRegister = $"{context} - {message}";

            if (!string.IsNullOrEmpty(obj))
            {
                if (string.IsNullOrWhiteSpace(obj))
                    return;

                try
                {
                    // Parse the JSON string
                    JsonConvert.DeserializeObject<object>(obj);
                    logRegister += Environment.NewLine + obj;
                }
                catch (JsonReaderException)
                {
                    // Invalid JSON
                }
                catch (Exception)
                {
                    // Other exceptions (e.g., null, etc.)
                }
            }

            Log.Information(logRegister);
        }

        public static void AddLog(string guid, HttpRequest request, string body)
        {
            AddLog($"{request.Method} - {guid}", request.Path, body);
        }

        public static void AddLog(string guid, HttpResponse response, string body)
        {
            AddLog($"{response.StatusCode} - {guid}", string.Empty, body);
        }
    }
}
