using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly EcommerceContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IPaymentRepository paymentRepository, EcommerceContext context, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> AddPaymentAsync(PaymentDto payment)
        {
            var newPayment = new Payment
            {
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                CreatedAt = payment.CreatedAt,
                Amount = payment.Amount,
                OrderId = payment.OrderId,
                PaymentMethodText = payment.PaymentMethod.ToString(),
                PaymentStatusText = payment.PaymentStatus.ToString(),
            };
            try
            {
                await _paymentRepository.AddPaymentAsync(newPayment);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Data = 0, Message = "Payment added", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Data = 0, Message = "", Status = false };
            }

        }

        public async Task<ApiResponse<int>> DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return new ApiResponse<int> { Data = id, Message = "No Payment found", Status = false };
            }
            try
            {
                await _paymentRepository.DeletePaymentAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Data = id, Message = "Payment deleted", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Data = id, Message = ex.Message, Status = false };
            }

        }

        public async Task<ApiResponse<IEnumerable<PaymentDto>>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();
            if (payments == null)
            {
                return new ApiResponse<IEnumerable<PaymentDto>> { Data = null, Message = "No Payment found", Status = false };
            }
            return new ApiResponse<IEnumerable<PaymentDto>>
            {
                Data = payments.Select(x => new PaymentDto
                {
                    PaymentMethod = x.PaymentMethod,
                    PaymentStatus = x.PaymentStatus,
                    CreatedAt = x.CreatedAt,
                    Amount = x.Amount,
                    OrderId = x.OrderId
                }),
                Message = "Payment found",
                Status = true
            };
        }

        public async Task<ApiResponse<PaymentDto>> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return new ApiResponse<PaymentDto> { Data = null, Message = "Payment not found", Status = false };
            }
            return new ApiResponse<PaymentDto>
            {
                Data = new PaymentDto
                {
                    PaymentMethod = payment.PaymentMethod,
                    PaymentStatus = payment.PaymentStatus,
                    CreatedAt = payment.CreatedAt,
                    Amount = payment.Amount,
                    OrderId = payment.OrderId
                },
                Message = "Payment found",
                Status = true
            };
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            var payment = await _context.Payments.Include(u => u.Order).ThenInclude(i => i.OrderItems).FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (payment == null)
            {
                return null;
            }
            return payment;

        }


        public async Task<ApiResponse<int>> UpdatePaymentAsync(int id, PaymentDto payment)
        {
            var paymentEx = await _paymentRepository.GetPaymentByIdAsync(id);
            try
            {
                paymentEx.PaymentMethod = payment.PaymentMethod;
                paymentEx.PaymentStatus = payment.PaymentStatus;
                payment.Amount = payment.Amount;
                payment.UpdatedAt = DateTime.UtcNow.Date;
                await _paymentRepository.UpdatePaymentAsync(id, paymentEx);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Data = 0, Message = "Payment updated", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Data = 0, Message = ex.Message, Status = false };
            }
        }
    }
}
