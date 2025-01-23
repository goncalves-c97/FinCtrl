using Newtonsoft.Json;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebLibrary.Exceptions;
using WebLibrary.Models;

namespace WebLibrary
{
    public static class HttpRequestManager
    {
        /// <summary>
        /// Retorna a conversão do content de uma requisição HTTP para o tipo desejado
        /// </summary>
        /// <typeparam name="T">Tipo desejado para retorno</typeparam>
        /// <param name="content">Content de requisição HTTP em string</param>
        /// <returns>Item correspondente ao tipo passado como referência</returns>
        /// <exception cref="HttpContentParseException"></exception>
        public static async Task<T> GetContentAsync<T>(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new UnsuccessfulResponseException(GetRequisitionFailureMessage(response, content));

            if (string.IsNullOrEmpty(content))
                throw new HttpContentParseException("Nenhum content a ser convertido.");

            if (typeof(T) == typeof(int))
            {
                if (int.TryParse(content, out int value))
                    return (T)(object)value;
            }
            else if (typeof(T) == typeof(long))
            {
                if (long.TryParse(content, out long value))
                    return (T)(object)value;
            }
            else if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(content, out bool value))
                    return (T)(object)value;
            }
            else
                return JsonConvert.DeserializeObject<T>(content)!;

            throw new HttpContentParseException($"Falha na conversão do content para {typeof(T).Name}");
        }

        public static async Task<T> GetContentAsync<T>(HttpRequestSetup request, AsyncPolicyWrap asyncPolicyWrap)
        {
            return await GetContentAsync<T>(await GetResponseAsync(request, asyncPolicyWrap));
        }

        public static T GetContentByResponseViewModel<T>(StandardHttpResponse responseViewModel)
        {
            if (responseViewModel == null)
                throw new UnsuccessfulResponseException();

            if (responseViewModel.Success)
            {
                if (typeof(T) == typeof(int) && int.TryParse(responseViewModel.Data.ToString(), out int intValue))
                    return (T)(object)intValue;
                else if (typeof(T) == typeof(long) && long.TryParse(responseViewModel.Data.ToString(), out long longValue))
                    return (T)(object)longValue;
                else if (typeof(T) == typeof(bool) && bool.TryParse(responseViewModel.Data.ToString(), out bool boolValue))
                    return (T)(object)boolValue;
                else if (typeof(T) == typeof(string))
                    return (T)(object)responseViewModel.Data.ToString()!;
                else
                {
                    string jsonObject = JsonConvert.SerializeObject(responseViewModel.Data);
                    return JsonConvert.DeserializeObject<T>(jsonObject)!;
                }
            }
            else
                throw new UnsuccessfulResponseException(responseViewModel.Message);

        }

        private static async Task<HttpResponseMessage> GetResponseAsync(HttpRequestSetup request, AsyncPolicyWrap asyncPolicyWrap)
        {
            try
            {
                return await asyncPolicyWrap.ExecuteAsync(() => SendHttpRequestAsync(request));
            }
            catch (Exception ex)
            {
                throw new UnsuccessfulResponseException(GetRequisitionExceptionMessage(request), ex);
            }
        }

        private static string GetRequisitionExceptionMessage(HttpRequestSetup request)
        {
            return $"Falha ao requisitar '{request}'.";
        }

        private static string GetRequisitionFailureMessage(HttpResponseMessage response, string content)
        {
            content = !string.IsNullOrEmpty(content) ? content : "*** vazio ***";
            return $"Houve alguma falha na requisição ao servidor.{Environment.NewLine}{Environment.NewLine}" +
                $"Request: {response.RequestMessage!.RequestUri!.AbsolutePath}{Environment.NewLine}" +
                $"Status : {(int)response.StatusCode} - {response.StatusCode}{Environment.NewLine}" +
                $"Content: {content}";
        }

        private static async Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestSetup request)
        {
            var req = new HttpRequestMessage(request.Method, request.FullUrl);

            if (request.HasBody)
                req.Content = new StringContent(JsonConvert.SerializeObject(request.Body), Encoding.UTF8, "application/json");

            if (request.HasToken)
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.Token);

            return await new HttpClient().SendAsync(req);
        }
    }
}
