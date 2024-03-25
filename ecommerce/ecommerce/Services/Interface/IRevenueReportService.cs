using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IRevenueReportService
    {
        Task<ApiResponse<IEnumerable<RevenueReportDto>>> GetAllRevenueReportsAsync();
        Task<ApiResponse<RevenueReportDto>> GetRevenueReportByIdAsync(int id);
        Task<ApiResponse<int>> AddRevenueReportAsync(RevenueReportDto revenueReport);
        Task<ApiResponse<int>> DeleteRevenueReportAsync(int id);
        Task<ApiResponse<int>> UpdateRevenueReportAsync(int id, RevenueReportDto revenueReport);
    }
}
