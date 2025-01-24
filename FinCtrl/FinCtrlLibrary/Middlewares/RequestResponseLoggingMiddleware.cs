using FinCtrlLibrary.Services;
using Microsoft.AspNetCore.Http;

namespace FinCtrlLibrary.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string guid = Guid.NewGuid().ToString();

            // Log the Request
            await LogRequest(context, guid);

            // Log the Response
            await LogResponse(context, guid);
        }

        public async Task LogRequest(HttpContext context, string guid)
        {
            string body = string.Empty;

            try
            {
                // Check if the request has a body
                if (context.Request.ContentLength == null || context.Request.ContentLength == 0)
                    return;

                // Enable buffering to read the body multiple times
                context.Request.EnableBuffering();

                // Read the request body
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);

                body = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0; // Reset position for downstream processing
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the request body: {ex.Message}");
            }
            finally
            {
                LogService.AddLog(guid, context.Request, body);

                // Proceed to the next middleware or endpoint
                await _next(context);
            }
        }

        private async Task LogResponse(HttpContext context, string guid)
        {
            string? body = string.Empty;

            try
            {
                // Backup the original response body stream
                var originalBodyStream = context.Response.Body;

                try
                {
                    // Create a new memory stream to capture the response body
                    using var responseBodyStream = new MemoryStream();
                    context.Response.Body = responseBodyStream;

                    // Call the next middleware in the pipeline
                    //await _next(context); // TODO: Verificar bug de chamada dupla à rota ao executar essa linha

                    // Reset the response body stream position and read it
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    body = await new StreamReader(responseBodyStream).ReadToEndAsync();
                    responseBodyStream.Seek(0, SeekOrigin.Begin);

                    // Copy the contents of the new stream back to the original stream
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                }
                finally
                {
                    // Restore the original response body stream
                    context.Response.Body = originalBodyStream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the response body: {ex.Message}");
            }
            finally
            {
                LogService.AddLog(guid, context.Response, body);
            }
        }
    }
}
