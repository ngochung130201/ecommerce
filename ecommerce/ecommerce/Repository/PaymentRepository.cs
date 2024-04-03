using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IRepositoryBase<Payment> _repositoryBase;
        private readonly EcommerceContext _context;
        public PaymentRepository(IRepositoryBase<Payment> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
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

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync(PagingForPayment? paging = null)
        {
            var payments = _context.Payments.Include(u=>u.Order).ThenInclude(u=>u.User).AsQueryable();
            if (paging == null)
            {
                return await payments.ToListAsync();
            }
            if (!string.IsNullOrEmpty(paging.UserName))
            {
                payments = payments.Where(u => u.Order.User.Email.Contains(paging.UserName) || u.Order.User.Username.Contains(paging.UserName));
            }
            if (paging.MinTotalPrice > 0)
            {
                payments = payments.Where(u => u.Amount >= paging.MinTotalPrice);
            }
            if (paging.MaxTotalPrice > 0)
            {
                payments = payments.Where(u => u.Amount <= paging.MaxTotalPrice);
            }
            if (paging.SortByDate)
            {
                payments = payments.OrderBy(u => u.CreatedAt);
            }
            else
            {
                payments = payments.OrderByDescending(u => u.CreatedAt);
            }
            if(paging.PaymentStatus != null)
            {
                payments = payments.Where(u => u.PaymentStatus == paging.PaymentStatus);
            }
            if(paging.PaymentMethod != null)
            {
                payments = payments.Where(u => u.PaymentMethod == paging.PaymentMethod);
            }
            return await payments.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize).ToListAsync();
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
