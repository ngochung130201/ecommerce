using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class RevenueReportService : IRevenueReportService
    {
        private readonly IRepositoryBase<RevenueReport> _revenueReportRepository;
        public RevenueReportService(IRepositoryBase<RevenueReport> revenueReportRepository)
        {
            _revenueReportRepository = revenueReportRepository;
        }

        public async Task<ApiResponse<int>> AddRevenueReportAsync(RevenueReportDto revenueReport)
        {
            var newRevenueReport = new RevenueReport
            {
                Date = revenueReport.Date,
                TotalRevenue = revenueReport.TotalRevenue
            };
            try
            {
                _revenueReportRepository.Create(newRevenueReport);
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Revenue Report added",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<int>> DeleteRevenueReportAsync(int id)
        {
            var revenueReport = await _revenueReportRepository.FindByIdAsync(id);
            if (revenueReport == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No Revenue Report found",
                    Status = false
                };
            }
            try
            {
                _revenueReportRepository.Delete(revenueReport);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Revenue Report deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<RevenueReportDto>>> GetAllRevenueReportsAsync()
        {
            var revenueReports = await _revenueReportRepository.FindAllAsync();
            if (revenueReports == null)
            {
                return new ApiResponse<IEnumerable<RevenueReportDto>>
                {
                    Data = null,
                    Message = "No Revenue Report found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<RevenueReportDto>>
            {
                Data = revenueReports.Select(x => new RevenueReportDto
                {
                    Date = x.Date,
                    TotalRevenue = x.TotalRevenue,
                    ReportId = x.ReportId,
                }),
                Message = "Revenue Report found",
                Status = true
            };
        }

        public async Task<ApiResponse<RevenueReportDto>> GetRevenueReportByIdAsync(int id)
        {
            var revenueReport = await _revenueReportRepository.FindByIdAsync(id);
            if (revenueReport == null)
            {
                return new ApiResponse<RevenueReportDto>
                {
                    Data = null,
                    Message = "Revenue Report not found",
                    Status = false
                };
            }
            return new ApiResponse<RevenueReportDto>
            {
                Data = new RevenueReportDto
                {
                    Date = revenueReport.Date,
                    TotalRevenue = revenueReport.TotalRevenue,
                    ReportId = revenueReport.ReportId,
                },
                Message = "Revenue Report found",
                Status = true
            };
        }

        public Task<ApiResponse<int>> UpdateRevenueReportAsync(int id, RevenueReportDto revenueReport)
        {
            var revenueReportToUpdate = new RevenueReport
            {
                Date = revenueReport.Date,
                TotalRevenue = revenueReport.TotalRevenue
            };
            try
            {
                _revenueReportRepository.Update(revenueReportToUpdate);
                return Task.FromResult(new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Revenue Report updated",
                    Status = true
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                });
            }
        }
    }
}
