using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<ApiResponse<int>> AddPaymentAsync(PaymentDto payment)
        {
            var newPayment = new Payment
            {
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                CreatedAt = payment.CreatedAt,
                Amount = payment.Amount,
                OrderId = payment.OrderId
            };
            try
            {
                await _paymentRepository.AddPaymentAsync(newPayment);
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

        public async Task<ApiResponse<int>> UpdatePaymentAsync(int id, PaymentDto payment)
        {
            var paymentToUpdate = new Payment
            {
                PaymentMethod = payment.PaymentMethod,
                PaymentStatus = payment.PaymentStatus,
                CreatedAt = payment.CreatedAt,
                Amount = payment.Amount,
                OrderId = payment.OrderId
            };
            try
            {
                await _paymentRepository.UpdatePaymentAsync(id, paymentToUpdate);
                return new ApiResponse<int> { Data = 0, Message = "Payment updated", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Data = 0, Message = ex.Message, Status = false };
            }
        }
    }
}
