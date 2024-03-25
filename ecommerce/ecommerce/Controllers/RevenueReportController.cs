using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
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
        [HttpGet]
        public async Task<IActionResult> GetAllRevenueReportsAsync()
        {
            var revenueReports = await _revenueReportService.GetAllRevenueReportsAsync();
            if (revenueReports.Status)
            {
                return Ok(revenueReports);
            }
            return BadRequest(revenueReports);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRevenueReportByIdAsync(int id)
        {
            var revenueReport = await _revenueReportService.GetRevenueReportByIdAsync(id);
            if (revenueReport.Status)
            {
                return Ok(revenueReport);
            }
            return BadRequest(revenueReport);
        }
        [HttpPost]
        public async Task<IActionResult> AddRevenueReportAsync(RevenueReportDto revenueReport)
        {
            var result = await _revenueReportService.AddRevenueReportAsync(revenueReport);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRevenueReportAsync(int id)
        {
            var result = await _revenueReportService.DeleteRevenueReportAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRevenueReportAsync(int id, RevenueReportDto revenueReport)
        {
            var result = await _revenueReportService.UpdateRevenueReportAsync(id, revenueReport);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
