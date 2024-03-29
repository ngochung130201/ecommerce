using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/revenue-report")]
    [ApiController]
    public class RevenueReportController : ControllerBase
    {
        private readonly IRevenueReportService _revenueReportService;
        public RevenueReportController(IRevenueReportService revenueReportService)
        {
            _revenueReportService = revenueReportService;
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetRevenueStats([FromQuery] RevenueStatsType type)
        {
            var result = await _revenueReportService.GetRevenueStatsTypeAsync(type);
            if (!result.Any())
            {
                return BadRequest();
            }
            return Ok(result);
        }
        // Get revenue report by range date
        [HttpGet("range")]
        public async Task<IActionResult> GetRevenueReportByRangeDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _revenueReportService.GetRevenueReportByDateRangeAsync(startDate, endDate);
            if (!result.Any())
            {
                return BadRequest();
            }
            return Ok(result);
        }
        // delete revenue report
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRevenueReport(int id)
        {
            var result = await _revenueReportService.DeleteRevenueReportAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // get detail revenue report
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRevenueReportById(int id)
        {
            var result = await _revenueReportService.GetRevenueReportByIdAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}