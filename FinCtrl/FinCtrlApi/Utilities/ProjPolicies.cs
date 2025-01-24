using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Polly;
using FinCtrlLibrary.Services;
using Serilog.Events;
using FinCtrlApi.Databases.MongoDb.FinCtrl.Repositories;

namespace FinCtrlApi.Utilities
{
    public static class ProjPolicies
    {
        private const int _databaseRetryCount = 3;  // 3 tentativas
        private const int _httpRetryCount = 3;  // 3 tentativas
        private const int _httpRequestTimeout = 30; // 30 segundos

        private static AsyncRetryPolicy? _databaseExceptionRetryPolicy;
        private static AsyncRetryPolicy? _httpRequestExceptionRetryPolicy;
        private static AsyncRetryPolicy? _httpRequestTimeoutRetryPolicy;
        private static AsyncTimeoutPolicy? _httpRequestTimeoutPolicy;

        private static AsyncPolicyWrap? _httpPolicyWrap;

        public static AsyncRetryPolicy RetryPolicyOnDatabaseException
        {
            get
            {
                _databaseExceptionRetryPolicy ??= Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: _databaseRetryCount,
                        sleepDurationProvider: GetExponentialTimeoutByAttempt,
                        onRetry: (exception, timeSpan, retryCount, context) =>
                        {
                            LogService.AddLog("Falha na interação com banco de dados", $"Tentativa {retryCount} para o contexto '{context.GetType().Name}'. Erro: {exception.Message}. Tentando novamente em {timeSpan} segundo(s).", LogEventLevel.Warning);
                        });

                return _databaseExceptionRetryPolicy;
            }
        }

        public static AsyncRetryPolicy RetryPolicyOnHttpRequestException
        {
            get
            {
                _httpRequestExceptionRetryPolicy ??= Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        retryCount: _httpRetryCount,
                        sleepDurationProvider: GetExponentialTimeoutByAttempt,
                        onRetry: (exception, timeSpan, retryCount) =>
                        {
                            LogService.AddLog("Exceção na requisição", $"Tentativa {retryCount} - erro na requisição HTTP: {exception.Message}. Tentando novamente em {timeSpan} segundo(s).", LogEventLevel.Warning);
                        });

                return _httpRequestExceptionRetryPolicy;
            }
        }

        public static AsyncRetryPolicy? RetryPolicyOnHttpRequestTimeout
        {
            get
            {
                _httpRequestTimeoutRetryPolicy ??= Policy
                    .Handle<TimeoutRejectedException>()
                    .WaitAndRetryAsync(
                        retryCount: _httpRetryCount,
                        sleepDurationProvider: GetExponentialTimeoutByAttempt,
                        onRetry: (timeSpan, retryCount) =>
                        {
                            LogService.AddLog("Timeout atingido", $"Tentativa {retryCount} - timeout na requisição HTTP. Tentando novamente em {timeSpan} segundo(s).", LogEventLevel.Warning);
                        });

                return _httpRequestExceptionRetryPolicy;
            }
        }

        public static AsyncTimeoutPolicy TimeoutPolicyOnHttpRequest
        {
            get
            {
                _httpRequestTimeoutPolicy ??= Policy.TimeoutAsync(_httpRequestTimeout);
                return _httpRequestTimeoutPolicy;
            }
        }

        public static AsyncPolicyWrap HttpPolicyWrap
        {
            get
            {
                _httpPolicyWrap ??= Policy.WrapAsync(RetryPolicyOnHttpRequestException, RetryPolicyOnHttpRequestTimeout, TimeoutPolicyOnHttpRequest);
                return _httpPolicyWrap;
            }
        }

        public static async Task ExecuteWithRetryAsync(Func<Task> action)
        {
            await RetryPolicyOnDatabaseException.ExecuteAsync(action);
        }

        public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action)
        {
            return await RetryPolicyOnDatabaseException.ExecuteAsync(action);
        }

        private static TimeSpan GetExponentialTimeoutByAttempt(int attempt) => TimeSpan.FromSeconds(Math.Pow(2, attempt));
    }
}
