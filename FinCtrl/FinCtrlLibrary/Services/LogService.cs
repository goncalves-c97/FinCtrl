using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace FinCtrlLibrary.Services
{
    public static class LogService
    {
        public static void AddLog(string context, string message, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            string logInfo = $"{context} - {message}";
            RegisterLog(logEventLevel, logInfo);
        }

        public static void AddLog(string context, string message, object obj, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            string logInfo = $"{context} - {message}";
            logInfo += JsonConvert.SerializeObject(obj);
            RegisterLog(logEventLevel, logInfo);
        }

        public static void AddLog(string context, string message, string obj, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            string logInfo = $"{context} - {message}";

            if (!string.IsNullOrEmpty(obj))
            {
                if (string.IsNullOrWhiteSpace(obj))
                    return;

                try
                {
                    // Parse the JSON string
                    JsonConvert.DeserializeObject<object>(obj);
                    logInfo += Environment.NewLine + obj;
                }
                catch (JsonReaderException)
                {
                    RegisterLog(LogEventLevel.Error, $"{logInfo} - falha na desserialização do json - JsonReaderException");
                    return;
                }
                catch (Exception)
                {
                    RegisterLog(LogEventLevel.Error, $"{logInfo} - falha na desserialização do json - Exception");
                    return;
                }
            }

            RegisterLog(logEventLevel, logInfo);
        }

        public static void AddLog(string guid, HttpRequest request, string body, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            AddLog($"{request.Method} - {guid}", request.Path, body, logEventLevel);
        }

        public static void AddLog(string guid, HttpResponse response, string body, LogEventLevel logEventLevel = LogEventLevel.Information)
        {
            AddLog($"{response.StatusCode} - {guid}", string.Empty, body, logEventLevel);
        }

        private static void RegisterLog(LogEventLevel logEventLevel, string logInfo)
        {
            switch (logEventLevel)
            {
                case LogEventLevel.Verbose:
                    Log.Verbose(logInfo);
                    break;
                case LogEventLevel.Debug:
                    Log.Debug(logInfo);
                    break;
                case LogEventLevel.Warning:
                    Log.Warning(logInfo);
                    break;
                case LogEventLevel.Error:
                    Log.Error(logInfo);
                    break;
                case LogEventLevel.Fatal:
                    Log.Fatal(logInfo);
                    break;
                default:
                    Log.Information(logInfo);
                    break;
            }
        }
    }
}
