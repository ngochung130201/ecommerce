using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPaymentsAsync()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            if (payments.Status)
            {
                return Ok(payments);
            }
            return BadRequest(payments);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment.Status)
            {
                return Ok(payment);
            }
            return BadRequest(payment);
        }
        [HttpPost]
        public async Task<IActionResult> AddPaymentAsync(PaymentDto payment)
        {
            var result = await _paymentService.AddPaymentAsync(payment);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentAsync(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePaymentAsync(int id, PaymentDto payment)
        {
            var result = await _paymentService.UpdatePaymentAsync(id, payment);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
             // Get payment by order id
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderIdAsync(int orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment != null)
            {
                return Ok(payment);
            }
            return BadRequest(payment);
        }
    }
}
