namespace ecommerce.Middleware
{
    public class CustomException : Exception
    {
        // Custom exception class message and status code properties
        public string? Message { get; set; } = null;
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public CustomException(string message, int statusCode, bool isSuccess = false)
        {
            Message = message;
            StatusCode = statusCode;
            IsSuccess = isSuccess;
        }
    }
}
