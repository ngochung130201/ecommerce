using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IRepositoryBase<Payment> _repositoryBase;
        public PaymentRepository(IRepositoryBase<Payment> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            try
            {
                _repositoryBase.Create(payment);

            }
            catch (Exception ex)
            {
                throw new CustomException("Payment not added", 500);
            }
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _repositoryBase.FindByIdAsync(id);
            if (payment == null)
            {
                throw new CustomException("No Payment found", 404);
            }
            try
            {
                _repositoryBase.Delete(payment);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            var payments = await _repositoryBase.FindAllAsync();
            if (payments == null)
            {
                throw new CustomException("No Payment found", 404);
            }
            return payments;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            var payment = await _repositoryBase.FindByIdAsync(id);
            if (payment == null)
            {
                throw new CustomException("Payment not found", 404);
            }
            return payment;
        }

        public async Task UpdatePaymentAsync(int id, Payment payment)
        {
            var paymentToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (paymentToUpdate == null)
            {
                throw new CustomException("Payment not found", 404);
            }
            try
            {
                _repositoryBase.Update(payment);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }
    }
}
