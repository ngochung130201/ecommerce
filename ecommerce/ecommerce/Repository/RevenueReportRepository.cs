using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class RevenueReportRepository : IRevenueReportRepository
    {
        private readonly IRepositoryBase<RevenueReport> _repositoryBase;
        public RevenueReportRepository(IRepositoryBase<RevenueReport> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }
        public async Task AddRevenueReportAsync(RevenueReport revenueReport)
        {
            try
            {
                _repositoryBase.Create(revenueReport);

            }
            catch (Exception ex)
            {
                throw new CustomException("Revenue Report not added", 500);
            }
        }

        public async Task DeleteRevenueReportAsync(int id)
        {
            try
            {
                var revenueReport = await _repositoryBase.FindByIdAsync(id);
                if (revenueReport == null)
                {
                    throw new CustomException("No Revenue Report found", 404);
                }
                _repositoryBase.Delete(revenueReport);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<RevenueReport>> GetAllRevenueReportsAsync()
        {
            var revenueReports = await _repositoryBase.FindAllAsync();
            if (revenueReports == null)
            {
                throw new CustomException("No Revenue Report found", 404);
            }
            return revenueReports;
        }

        public async Task<RevenueReport> GetRevenueReportByIdAsync(int id)
        {
            var revenueReport = await _repositoryBase.FindByIdAsync(id);
            if (revenueReport == null)
            {
                throw new CustomException("Revenue Report not found", 404);
            }
            return revenueReport;
        }

        public async Task UpdateRevenueReportAsync(int id, RevenueReport revenueReport)
        {
            var revenueReportToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (revenueReportToUpdate == null)
            {
                throw new CustomException("No Revenue Report found", 404);
            }
            revenueReportToUpdate.TotalRevenue = revenueReport.TotalRevenue;
            revenueReportToUpdate.UpdatedAt = DateTime.UtcNow;
            try
            {
                _repositoryBase.Update(revenueReportToUpdate);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }
    }
}
