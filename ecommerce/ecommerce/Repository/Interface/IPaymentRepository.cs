using ecommerce.Models;

namespace ecommerce.Repository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment> GetPaymentByIdAsync(int id);
        Task AddPaymentAsync(Payment payment);
        Task DeletePaymentAsync(int id);
        Task UpdatePaymentAsync(int id, Payment payment);
    }
}
