using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IPaymentService
    {
        Task<ApiResponse<IEnumerable<PaymentDto>>> GetAllPaymentsAsync();
        Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(int id);
        Task<ApiResponse<int>> AddPaymentAsync(PaymentDto payment);
        Task<ApiResponse<int>> DeletePaymentAsync(int id);
        Task<ApiResponse<int>> UpdatePaymentAsync(int id, PaymentDto payment);
    }
}
