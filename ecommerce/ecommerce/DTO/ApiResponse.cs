namespace ecommerce.DTO
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public string? Message { get; set; } = null;
        public T? Data { get; set; }
        public int Total { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPage { get; set; } = 0;

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
