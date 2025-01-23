using System.Text;

namespace WebLibrary.Models
{
    public sealed class HttpRequestSetup
    {
        public HttpMethod Method { get; private set; }
        public string Url { get; private set; }
        public string ControllerAction { get; private set; }
        public string[]? Parameters { get; private set; }
        public object? Body { get; set; }
        public string? Token { get; set; }

        public string FullUrl => $"{Url}/{ControllerAction}/{ParametersString}";
        public bool HasToken => !string.IsNullOrEmpty(Token);
        public bool HasBody => Body != null;

        private string ParametersString
        {
            get
            {
                if (Parameters == null)
                    return string.Empty;

                StringBuilder sb = new();

                foreach (string param in Parameters)
                    sb.Append(param + "/");

                return sb.ToString();
            }
        }

        public HttpRequestSetup(HttpMethod method, string url, string controller)
        {
            Method = method;
            Url = TrimDashes(url);
            ControllerAction = TrimDashes(controller);
        }

        public HttpRequestSetup(HttpMethod method, string url, string controller, params string[] parameters)
        {
            Method = method;
            Url = TrimDashes(url);
            ControllerAction = TrimDashes(controller);
            Parameters = parameters;
        }

        /// <summary>
        /// Remove as barras de uma entrada. Utilizada para garantir que a concatenação
        /// dos parâmetros da rota não gerem situações de barras duplicadas
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string TrimDashes(string input)
        {
            return input.Trim('/');
        }

        public override string ToString()
        {
            return $"{Method.Method} - {ControllerAction}";
        }
    }
}
