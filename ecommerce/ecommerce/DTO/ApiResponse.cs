namespace ecommerce.DTO
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; } = null;
        public T? Data { get; set; }
        public int Total { get; set; } = 0;
        public ApiResponse()
        {

        }
        public ApiResponse(bool status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }

    }

}
