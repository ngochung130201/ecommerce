using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IRevenueReportRepository
    {
        Task<IEnumerable<RevenueReport>> GetAllRevenueReportsAsync();
        Task<RevenueReport> GetRevenueReportByIdAsync(int id);
        Task AddRevenueReportAsync(RevenueReport revenueReport);
        Task DeleteRevenueReportAsync(int id);
        Task UpdateRevenueReportAsync(int id, RevenueReport revenueReport);
    }
}
