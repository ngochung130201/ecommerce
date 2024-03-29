using ecommerce.DTO;
using ecommerce.Enums;

namespace ecommerce.Services.Interface
{
    public interface IRevenueReportService
    {
        Task<ApiResponse<IEnumerable<RevenueReportDto>>> GetAllRevenueReportsAsync();
        Task<ApiResponse<RevenueReportDto>> GetRevenueReportByIdAsync(int id);
        Task<ApiResponse<int>> AddRevenueReportAsync(RevenueReportAddDto revenueReport);
        Task<ApiResponse<int>> DeleteRevenueReportAsync(int id);
        Task<ApiResponse<int>> UpdateRevenueReportAsync(int id, RevenueReportDto revenueReport);
        Task<List<object>> GetRevenueStatsTypeAsync(RevenueStatsType type);
        // Get revenue report by date range
        Task<IEnumerable<RevenueReportDto>> GetRevenueReportByDateRangeAsync(DateTime startDate, DateTime endDate);

    }
}