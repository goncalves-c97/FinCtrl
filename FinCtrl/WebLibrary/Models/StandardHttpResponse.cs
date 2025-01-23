namespace WebLibrary.Models
{
    public class StandardHttpResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } = null;
        public object? Data { get; set; } = default;
        public string? ErrorCode { get; set; } = null;

        public StandardHttpResponse()
        {

        }

        public StandardHttpResponse(bool success, object data)
        {
            Success = success;
            Data = data;
        }

        public static StandardHttpResponse NewResponse(bool success)
        {
            return new StandardHttpResponse
            {
                Success = success
            };
        }

        public static StandardHttpResponse NewResponse(bool success, object data)
        {
            return new StandardHttpResponse
            {
                Success = success,
                Data = data
            };
        }

        public static StandardHttpResponse NewResponse(bool success, object data, string message)
        {
            return new StandardHttpResponse
            {
                Success = success,
                Data = data,
                Message = message
            };
        }

        public static StandardHttpResponse NewUnsuccessfulResponse(string message)
        {
            return new StandardHttpResponse
            {
                Success = false,
                Message = message
            };
        }

        public static StandardHttpResponse NewExceptionResponse(Exception ex)
        {
            return new StandardHttpResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
