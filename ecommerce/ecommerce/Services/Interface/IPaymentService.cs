using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Services.Interface
{
    public interface IPaymentService
    {
        Task<ApiResponse<IEnumerable<PaymentDto>>> GetAllPaymentsAsync(PagingForPayment? paging = null);
        Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(int id);
        Task<ApiResponse<int>> AddPaymentAsync(PaymentDto payment);
        Task<ApiResponse<int>> DeletePaymentAsync(int id);
        Task<ApiResponse<int>> UpdatePaymentAsync(int id, PaymentDto payment);
        // Get payment by order id
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
    }
}
