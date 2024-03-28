using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders.Status)
            {
                return Ok(orders);
            }
            return BadRequest(orders);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order.Status)
            {
                return Ok(order);
            }
            return BadRequest(order);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderAsync(OrderRequestDto order)
        {
            var result = await _orderService.ProcessOrderAsync(order);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrderAsync(OrderUpdate order)
        {
            var result = await _orderService.UpdateOrderAsync(order);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // delete order by list order item id
        [HttpDelete("{orderId}/orderItem/{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItemsAsync(OrderItemDeleteDto orderItemDto)
        {
            var result = await _orderService.DeleteOrderItemsAsync(orderItemDto);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }


    }
}
